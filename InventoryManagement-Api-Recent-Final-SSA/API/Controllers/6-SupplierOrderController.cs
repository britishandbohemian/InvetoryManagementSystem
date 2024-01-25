using InventoryAPI.Data;
using InventoryAPI.DataTransferObjects.SupplierDto;
using InventoryAPI.Model.SupplierModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static OrderItem;
using static OrderProduct;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierOrdersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SupplierOrdersController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("supplier/{id}")]
        public ActionResult<Supplier> GetSupplier(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound($"Supplier with ID {id} not found.");
            }
            return supplier;
        }



        //Thus when that happens update the products supplier Id column with the supplier choosen
        [HttpPost("order-product")]
        public ActionResult<OrderProductDTO> OrderProduct(OrderProductDTO productOrderDto)
        {


            var product = _context.Products.Find(productOrderDto.ProductId);
            if (product == null)
            {
                return NotFound($"Product with ID {productOrderDto.ProductId} not found.");
            }

            OrderProduct orderProduct = new OrderProduct
            {
                //Take product Id Find Product And place its details inside the variable with the amount of units ordered for it
                ProductId = productOrderDto.ProductId,
                UnitsOrdered = productOrderDto.UnitsOrdered,
                TotalCostOfOrder = productOrderDto.TotalCostOfOrder,
                PiecesPerUnit = productOrderDto.PiecesPerUnit,
                OrderDate = productOrderDto.OrderDate
            };



            // After creating the order product, update the product's cost and selling price
            product.ProductCostPrice = orderProduct.CostPricePerPiece;

            // Use the existing markup to update the selling price
            product.ProductSellingPrice = product.ProductCostPrice * (1 + product.MarkupPercentage / 100);

            // Add the newly created orderProduct to the database context
            _context.OrderProducts.Add(orderProduct);

            // Save the changes to the database
            _context.SaveChanges();

            // After saving the changes to the database, simply return the DTO.
            return Ok(orderProduct);
        }
        [HttpPost("order-item")]
        public ActionResult<OrderItemDTO> OrderItem(OrderItemDTO itemOrderDto)
        {


            var item = _context.Items.Find(itemOrderDto.ItemId);
            if (item == null)
            {
                return NotFound($"Item with ID {itemOrderDto.ItemId} not found.");
            }

            OrderItem orderItem = new OrderItem
            {
                ItemId = itemOrderDto.ItemId,
                UnitsOrdered = itemOrderDto.UnitsOrdered,
                TotalCostOfOrder = itemOrderDto.TotalCostOfOrder,
                PiecesPerUnit = itemOrderDto.PiecesPerUnit,
                OrderDate = itemOrderDto.OrderDate
            };

            // Update the item's cost price
            item.PricePerPiece = orderItem.TotalCostOfOrder / (orderItem.UnitsOrdered * orderItem.PiecesPerUnit);

            // Add the newly created orderItem to the database context
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();

            // Find all products that have this item as a component
            var linkedProducts = _context.ProductItems.Where(p => p.ItemId == item.ItemId).Select(p => p.Product).ToList();

            // Update the selling price for these products based on the new item cost and markup percentage
            foreach (var product in linkedProducts)
            {
                product.ProductCostPrice += item.PricePerPiece; // Update product's cost price
                product.ProductSellingPrice = product.ProductCostPrice * (1 + product.MarkupPercentage / 100); // Recalculate selling price
            }

            // Save the changes to the database
            _context.SaveChanges();

            // Return the newly created order item details
            return Ok(orderItem);
        }

        [HttpPost("ConfirmOrder")]
        public IActionResult ConfirmOrder([FromBody] ConfirmOrderArrivalDTO orderDto)
        {
            try
            {
                // Validate the Supplier exists
                var supplier = _context.Suppliers.Find(orderDto.SupplierId);
                if (supplier == null)
                    return NotFound("Supplier not found.");

                // Fetch all Supplier Orders related to the specified supplier
                var supplierOrders = _context.SupplierOrders.Where(so => so.SupplierId == orderDto.SupplierId).ToList();
                if (!supplierOrders.Any())
                {
                    return NotFound("No supplier orders found for the specified supplier.");
                }

                // Find the specific SupplierOrder by its OrderId from the fetched orders
                var specificOrder = supplierOrders.FirstOrDefault(so => so.SupplierOrderId == orderDto.SupplierOrderId);
                if (specificOrder == null)
                {
                    return NotFound($"No supplier order found with the given OrderId: {orderDto.SupplierOrderId}.");
                }
                // Loop through each order and update its status to Confirmed
                foreach (var order in supplierOrders)
                {
                    order.Status = SupplierOrder.StatusOfOrder.Arrived;  // Change this line
                }

                // Update Products and Items
                UpdateProducts(orderDto);
                UpdateItems(orderDto);

                // Update Supplier Rating
                UpdateSupplierRating(orderDto, supplier);

                // Save changes to the database
                _context.SaveChanges();

                return Ok(new { message = "Orders confirmed successfully!" });
            }
            catch (Exception ex)
            {
                // Log exception if you have a logger
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        private void UpdateProducts(ConfirmOrderArrivalDTO orderDto)
        {
            var productIds = orderDto.ProductsReceived.Select(p => p.ProductId).ToList();
            var products = _context.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            foreach (var receivedProduct in orderDto.ProductsReceived)
            {

                if (receivedProduct.QuantityReceived == 0 && receivedProduct.SellByDate == null)
                {
                    continue;
                }

                var product = products.FirstOrDefault(p => p.ProductId == receivedProduct.ProductId);
                if (product == null) continue;

                product.UnitsInInventory += receivedProduct.QuantityReceived;
                product.SellByDate = receivedProduct.SellByDate;
            }
        }
        [HttpPost("ConfirmSupplierReceivedOrder/{supplierOrderId}")]
        public IActionResult ConfirmSupplierReceivedOrder(int supplierOrderId)
        {
            try
            {
                // Find the specific SupplierOrder by its OrderId
                var specificOrder = _context.SupplierOrders
                                             .Include(so => so.OrderProducts)
                                             .Include(so => so.OrderItems)
                                             .FirstOrDefault(so => so.SupplierOrderId == supplierOrderId);

                if (specificOrder == null)
                {
                    return NotFound($"No supplier order found with the given OrderId: {supplierOrderId}.");
                }

                // Check if the order has already been confirmed or arrived
                if (specificOrder.Status == SupplierOrder.StatusOfOrder.Confirmed ||
                    specificOrder.Status == SupplierOrder.StatusOfOrder.Arrived)
                {
                    return Ok(new { message = "The order is already confirmed or has arrived. No further action taken." });
                }

                // Change the status to EnRoute
                specificOrder.Status = SupplierOrder.StatusOfOrder.EnRoute;

                // Save changes to the database
                _context.SaveChanges();

                return Ok(new { message = "Order status updated to EnRoute successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception if you have a logger
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        private void UpdateItems(ConfirmOrderArrivalDTO orderDto)
        {
            var itemIds = orderDto.ItemsReceived.Select(i => i.ItemId).ToList();
            var items = _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToList();

            foreach (var receivedItem in orderDto.ItemsReceived)
            {

                if (receivedItem.QuantityReceived == 0 && receivedItem.SellByDate == null)
                {
                    continue;
                }


                var item = items.FirstOrDefault(i => i.ItemId == receivedItem.ItemId);
                if (item == null) continue;

                item.UnitsInInventory += receivedItem.QuantityReceived;
                item.SellByDate = receivedItem.SellByDate;
            }
        }

        private void UpdateSupplierRating(ConfirmOrderArrivalDTO orderDto, Supplier supplier)
        {
            var supplierOrders = _context.SupplierOrders.Where(so => so.SupplierId == orderDto.SupplierId).ToList();
            double totalRatings = orderDto.SupplierRating;
            int numberOfRatings = 1; // For the current order

            foreach (var order in supplierOrders)
            {
                totalRatings += order.Rating;
                numberOfRatings++;
            }

            double newAverageRating = totalRatings / numberOfRatings;
            supplier.Rating = Convert.ToInt32(Math.Round(newAverageRating));  // Update and round the supplier rating
        }

        // 4. Set the rating for a supplier when an order is confirmed
        [HttpPost("supplier-order/{orderId}/rate")]
        public ActionResult SetRatingForSupplierOrder(int orderId, [FromBody] int rating)
        {
            var supplierOrder = _context.SupplierOrders.Find(orderId);
            if (supplierOrder == null)
            {
                return NotFound($"Supplier Order with ID {orderId} not found.");
            }

            if (rating < 1 || rating > 10)  // Assuming rating scale is 1-10
            {
                return BadRequest("Rating must be between 1 and 10.");
            }

            supplierOrder.Rating = rating;
            _context.SaveChanges();

            return Ok($"Rating for supplier order {orderId} set to {rating}.");
        }



        [HttpGet("supplier-order-details/{supplierOrderId}")]
        public ActionResult<SupplierOrderDetailsDTO> GetSupplierOrderDetails(int supplierOrderId)
        {
            var supplierOrder = _context.SupplierOrders
                .Include(so => so.Supplier)  // Eager load the Supplier entity
                .Include(so => so.OrderProducts)
                    .ThenInclude(op => op.Product)  // Navigating to the actual product details
                .Include(so => so.OrderItems)
                    .ThenInclude(oi => oi.Item)  // Navigating to the actual item details
                .FirstOrDefault(so => so.SupplierOrderId == supplierOrderId);

            if (supplierOrder == null)
            {
                return NotFound(new { message = "No Orders" });
            }



            SupplierOrderDetailsDTO orderDetails = new SupplierOrderDetailsDTO
            {
                SupplierOrderId = supplierOrder.SupplierOrderId,
                TotalCostOfOrder = supplierOrder.TotalCostOfOrder,
                SupplierName = supplierOrder.SupplierName,  // Access SupplierName directly from supplierOrder

                LeadTimeFromSupplier = supplierOrder.LeadTimeFromSupplier,
                SupplierId = supplierOrder.SupplierId,
                OrderDate = supplierOrder.OrderDate,
                TrackingNumber = supplierOrder.TrackingNumber,
                ExpectedDeliveryDate = supplierOrder.ExpectedDeliveryDate,
                PlacedByUserId = (int)supplierOrder.PlacedByUserId,
                Status = supplierOrder.Status,

                OrderProducts = supplierOrder.OrderProducts.Select(op => new OrderedProductDetailsDTO
                {
                    OrderedProductId = op.OrderedProductId,
                    ProductId = op.Product.ProductId,
                    ProductName = op.Product.ProductName,
                    UnitsOrdered = op.UnitsOrdered,
                    TotalCostOfOrder = op.TotalCostOfOrder
                }).ToList(),

                OrderItems = supplierOrder.OrderItems.Select(oi => new OrderedItemDetailsDTO
                {
                    OrderedItemId = oi.OrderedItemId,
                    ItemId = oi.Item.ItemId,
                    ItemName = oi.Item.ItemName,
                    UnitsOrdered = oi.UnitsOrdered,
                    TotalCostOfOrder = oi.TotalCostOfOrder
                }).ToList()
            };

            return Ok(orderDetails);
        }

        [HttpGet("supplier-order-details")]
        public ActionResult<IEnumerable<SupplierOrderDetailsDTO>> GetAllSupplierOrderDetails()
        {
            var supplierOrders = _context.SupplierOrders
                .Include(so => so.OrderProducts)
                    .ThenInclude(op => op.Product)  // Navigating to the actual product details
                .Include(so => so.OrderItems)
                    .ThenInclude(oi => oi.Item)  // Navigating to the actual item details
                .ToList();

            if (!supplierOrders.Any())
            {
                return NotFound("No supplier orders found.");
            }

            var allOrderDetails = supplierOrders.Select(supplierOrder => new SupplierOrderDetailsDTO
            {
                SupplierOrderId = supplierOrder.SupplierOrderId,
                TotalCostOfOrder = supplierOrder.TotalCostOfOrder,
                LeadTimeFromSupplier = supplierOrder.LeadTimeFromSupplier,
                SupplierName = supplierOrder.SupplierName,  // Access SupplierName directly from supplierOrder

                SupplierId = supplierOrder.SupplierId,
                OrderDate = supplierOrder.OrderDate,
                TrackingNumber = supplierOrder.TrackingNumber,
                ExpectedDeliveryDate = supplierOrder.ExpectedDeliveryDate,
                PlacedByUserId = (int)supplierOrder.PlacedByUserId,  // Assuming PlacedByUserId is nullable in your model
                Status = supplierOrder.Status,
                OrderProducts = supplierOrder.OrderProducts.Select(op => new OrderedProductDetailsDTO
                {
                    OrderedProductId = op.OrderedProductId,
                    ProductId = op.Product.ProductId,
                    ProductName = op.Product.ProductName,
                    UnitsOrdered = op.UnitsOrdered,
                    TotalCostOfOrder = op.TotalCostOfOrder
                }).ToList(),
                OrderItems = supplierOrder.OrderItems.Select(oi => new OrderedItemDetailsDTO
                {
                    OrderedItemId = oi.OrderedItemId,
                    ItemId = oi.Item.ItemId,
                    ItemName = oi.Item.ItemName,
                    UnitsOrdered = oi.UnitsOrdered,
                    TotalCostOfOrder = oi.TotalCostOfOrder
                }).ToList()
            }).ToList();

            return Ok(allOrderDetails);
        }
        [HttpGet("order-items")]
        public ActionResult<IEnumerable<OrderedItemDetailsDTO>> GetAllOrderItems()
        {
            // Eagerly load related Item entities
            var orderItems = _context.OrderItems.Include(oi => oi.Item).ToList();

            if (!orderItems.Any())
            {
                return NotFound("No order items found.");
            }

            // Map OrderItem entities to DTOs
            var orderItemDTOs = orderItems.Select(oi => new OrderedItemDetailsDTO
            {
                OrderedItemId = oi.OrderedItemId,
                ItemId = oi.ItemId,
                ItemName = oi.Item.ItemName,  // Get the Item Name here
                UnitsOrdered = oi.UnitsOrdered,
                TotalCostOfOrder = oi.TotalCostOfOrder,
                Status = oi.Status
            }).ToList();

            return Ok(orderItemDTOs);
        }

        [HttpGet("order-products")]
        public ActionResult<IEnumerable<OrderedProductDetailsDTO>> GetAllOrderProducts()
        {
            // Eagerly load related Product entities
            var orderProducts = _context.OrderProducts.Include(op => op.Product).ToList();

            if (!orderProducts.Any())
            {
                return NotFound("No order products found.");
            }

            // Map OrderProduct entities to DTOs
            var orderProductDTOs = orderProducts.Select(op => new OrderedProductDetailsDTO
            {
                OrderedProductId = op.OrderedProductId,
                ProductId = op.ProductId,
                ProductName = op.Product.ProductName,  // Get the Product Name here
                UnitsOrdered = op.UnitsOrdered,
                TotalCostOfOrder = op.TotalCostOfOrder,
                Status = op.Status
            }).ToList();

            return Ok(orderProductDTOs);
        }

        private static Random _random = new Random();
        private int RandomNumber()
        {
            return _random.Next(1000, 10000); // This will give you a random number between 1000 and 9999
        }


        [HttpPost("create-supplier-order")]
        public ActionResult<CreateSupplierOrderDTO> CreateSupplierOrderForOrderedProductsAndItems(CreateSupplierOrderDTO orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Invalid order data.");
            }

            var supplier = _context.Suppliers.Find(orderDto.SupplierId);
            if (supplier == null)
            {
                return NotFound($"Supplier with ID {orderDto.SupplierId} not found.");
            }

            decimal totalCost = 0;
            // Update OrderProducts
            if (orderDto.OrderProductIds != null && orderDto.OrderProductIds.Any())
            {
                foreach (var id in orderDto.OrderProductIds)
                {
                    var op = _context.OrderProducts.Find(id);
                    if (op != null)
                    {
                        totalCost += op.TotalCostOfOrder;
                        op.Active = 1;
                        op.Status = 0;
                    }
                }
            }

            // Update OrderItems
            if (orderDto.OrderItemIds != null && orderDto.OrderItemIds.Any())
            {
                foreach (var id in orderDto.OrderItemIds)
                {
                    var oi = _context.OrderItems.Find(id);
                    if (oi != null)
                    {
                        totalCost += oi.TotalCostOfOrder;
                        oi.Active = 1;
                        oi.Status = 0;
                    }
                }
            }

            SupplierOrder supplierOrder = new SupplierOrder
            {
                TotalCostOfOrder = totalCost,
                LeadTimeFromSupplier = orderDto.LeadTimeFromSupplier,
                SupplierId = orderDto.SupplierId,
                SupplierName = supplier.SupplierName,  // Ensure the supplier's name is also placed
                OrderDate = DateTime.Now,
                TrackingNumber = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + orderDto.SupplierId + "_" + RandomNumber(),
                ExpectedDeliveryDate = orderDto.ExpectedDeliveryDate,
                PlacedByUserId = orderDto.PlacedByUserId,
                Status = SupplierOrder.StatusOfOrder.WaitingForSupplierToConfirm
            };

            _context.SupplierOrders.Add(supplierOrder);
            _context.SaveChanges();  // Save to generate the ID for supplierOrder

            // Associate OrderProducts with the new SupplierOrder
            if (orderDto.OrderProductIds != null && orderDto.OrderProductIds.Any())
            {
                foreach (var id in orderDto.OrderProductIds)
                {
                    var op = _context.OrderProducts.Find(id);
                    if (op != null)
                    {
                        op.SupplierOrderId = supplierOrder.SupplierOrderId;  // Associate with SupplierOrder
                        op.Active = 1;
                        op.Status = OrderStatus.Ordered;  // Change the status to Ordered
                    }
                }
            }

            // Associate OrderItems with the new SupplierOrder
            if (orderDto.OrderItemIds != null && orderDto.OrderItemIds.Any())
            {
                foreach (var id in orderDto.OrderItemIds)
                {
                    var oi = _context.OrderItems.Find(id);
                    if (oi != null)
                    {
                        oi.SupplierOrderId = supplierOrder.SupplierOrderId;  // Associate with SupplierOrder
                        oi.Active = 1;
                        oi.Status = OrderItemStatus.Ordered;  // Change the status to Ordered
                    }
                }
            }

            _context.SaveChanges();
            _context.SaveChanges();  // Save changes after associating products and items

            return Ok(supplierOrder);
        }

    }
}
