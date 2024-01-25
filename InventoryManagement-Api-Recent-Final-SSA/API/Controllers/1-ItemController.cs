using InventoryAPI.Data;
using InventoryAPI.DataTransferObjects.ItemDto;
using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.LinkModels;
using InventoryAPI.Model.SalesModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Controllers.ComponentController
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ItemsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("ItemImpact/{itemId}")]
        public async Task<IActionResult> GetItemImpact(int itemId)
        {
            try
            {
                // Get the count of products linked to this item
                int linkedProductCount = await _context.Set<ProductToItem>()
                                                       .Where(x => x.ItemId == itemId)
                                                       .CountAsync();

                // Get the total count of distinct items linked to any products
                int totalUniqueItems = await _context.Set<ProductToItem>()
                                                     .Select(x => x.ItemId)
                                                     .Distinct()
                                                     .CountAsync();

                if (totalUniqueItems == 0)
                {
                    return BadRequest("No items found linked to any products.");
                }

                // Calculate impact percentage
                decimal impactPercentage = ((decimal)linkedProductCount / totalUniqueItems) * 100;

                return Ok(new { ImpactPercentage = impactPercentage });
            }
            catch (System.Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    
    [HttpPost("CreateItem")]
        public async Task<ActionResult> CreateItem([FromBody] CreateItemDTO dto)
        {


            var item = new Item
            {
                ItemName = dto.ItemName,
                UnitOfMeasurement = dto.UnitOfMeasurement,
                SellByDate = dto.SellByDate,
                MinimumThreshold = dto.MinimumThreshold,
                MaximumThreshold = dto.MaximumThreshold,
                UnitsInInventory = dto.UnitsInInventory,
                Status = dto.Status
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
        }


        [HttpGet("GetAllItems")]
        public async Task<ActionResult<IEnumerable<ReadItemDto>>> GetItems()
        {
            var items = await _context.Items.ToListAsync();
            var today = DateTime.Today;

            foreach (var item in items)
            {
                if (item.SellByDate.HasValue && item.SellByDate.Value.Date <= today)
                {
                    var expirationData = await FetchItemExpirationData(item.ItemId);
                    expirationData.TimesExpired++;
                    expirationData.LastExpirationDate = today;
                    _context.ItemExpirationData.Update(expirationData);

                    // Update the item's status to Expired
                    item.Status = Item.ItemStatus.Expired;

                    // Log the expiration event (replace with your logging mechanism)
                    Console.WriteLine($"Item {item.ItemId} has expired.");
                }
            }


            foreach (var item in items)
            {
                if (item.SellByDate.HasValue && item.SellByDate.Value.Date <= today)
                {
                    var expirationData = await FetchItemExpirationData(item.ItemId);
                    expirationData.TimesExpired++;
                    expirationData.LastExpirationDate = today;
                    _context.ItemExpirationData.Update(expirationData);
                }
            }

            await _context.SaveChangesAsync();

            return items.Select(i => new ReadItemDto
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                UnitOfMeasurementName = i.UnitOfMeasurement.ToString(),
                SellByDate = i.SellByDate,
                UnitsInInventory = i.UnitsInInventory,
                PiecesPerUnit = i.PiecesPerUnit ?? 0,  // Default to 0 if null
                PricePerPiece = i.PricePerPiece ?? 0m, // Default to 0 if null
                MinimumThreshold = i.MinimumThreshold,
                MaximumThreshold = i.MaximumThreshold,
                SupplierId = i.SupplierId ?? 0,        // Default to 0 if null
                Status = i.Status
            }).ToList();
        }


        // Fetch a single item by its ID
        [HttpGet("GetItem/{id}")]
        public async Task<ActionResult<ReadItemDto>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            // Update expiration data if the item is expired
            var today = DateTime.Today;
            if (item.SellByDate.HasValue && item.SellByDate.Value.Date <= today)
            {
                var expirationData = await FetchItemExpirationData(item.ItemId);
                expirationData.TimesExpired++;
                expirationData.LastExpirationDate = today;
                _context.ItemExpirationData.Update(expirationData);
                await _context.SaveChangesAsync();
            }

            return new ReadItemDto
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                UnitOfMeasurementName = item.UnitOfMeasurement.ToString(),
                SellByDate = item.SellByDate,
                UnitsInInventory = item.UnitsInInventory,
                PiecesPerUnit = item.PiecesPerUnit ?? 0,  // Default to 0 if null
                PricePerPiece = item.PricePerPiece ?? 0m, // Default to 0 if null
                MinimumThreshold = item.MinimumThreshold,
                MaximumThreshold = item.MaximumThreshold,
                SupplierId = item.SupplierId ?? 0,        // Default to 0 if null
                Status = item.Status
            };
        }


        [HttpPut("UpdateItem/{id}")]
        public IActionResult UpdateItem(int id, UpdateItemDTO dto)
        {
            var item = _context.Items.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            item.ItemName = dto.ItemName;
            item.UnitOfMeasurement = dto.UnitOfMeasurement;
            item.SellByDate = dto.SellByDate;
            item.MinimumThreshold = dto.MinimumThreshold;
            item.MaximumThreshold = dto.MaximumThreshold;
            item.UnitsInInventory = dto.UnitsInInventory;
            item.Status = dto.Status;

            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("DeleteItemWithId/{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.Items.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return NoContent();
        }



        //Basic Functions Above




        [HttpGet("most-used-items")]
        public IActionResult GetMostUsedItemsInProducts([FromQuery] int topN = 10)
        {
            if (topN <= 0)
            {
                return BadRequest("Please provide a valid number for topN.");
            }

            // Fetch the ProductItems along with their corresponding Items from the database
            var productItemsWithItems = _context.ProductItems
                                                .Include(pi => pi.Item)
                                                .ToList();

            // Group by ItemId and calculate how many products each item is used in
            var mostUsedItems = productItemsWithItems
                                .GroupBy(pi => pi.ItemId)
                                .Select(g => new
                                {
                                    ItemId = g.Key,
                                    ItemName = g.First().Item.ItemName, // Assuming that the first entry in the group will have the valid Item
                                    ProductsCount = g.Count(),
                                    UnitsInInventory = g.First().Item.UnitsInInventory,
                                    MinimumThreshold = g.First().Item.MinimumThreshold,
                                    MaximumThreshold = g.First().Item.MaximumThreshold
                                })
                                .OrderByDescending(x => x.ProductsCount)
                                .Take(topN)
                                .ToList();

            return Ok(mostUsedItems);
        }

        private async Task<int> FetchTotalNumberOfProducts()
        {
            return await _context.Products.CountAsync();
        }


        public class ItemSalesData
        {
            public int TotalUnitsSold { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TotalProfit { get; set; }
        }

        [HttpGet("StockOutRiskIndicator")]
        public async Task<ActionResult<object>> GetStockOutRiskIndicator()
        {
            // Fetch all items from the database
            List<Item> allItems = await _context.Items.ToListAsync();
            int totalNumberOfItems = allItems.Count;

            // Calculate the number of items below the minimum threshold
            int itemsBelowMinThreshold = allItems.Count(item => item.UnitsInInventory < item.MinimumThreshold);

            // Calculate the Stock-out Risk Indicator
            decimal stockOutRiskIndicator = 0;
            string stockOutRiskStatus = "Good";

            if (totalNumberOfItems != 0)
            {
                stockOutRiskIndicator = ((decimal)itemsBelowMinThreshold / totalNumberOfItems) * 100;

                if (stockOutRiskIndicator > 30) // Hypothetical threshold for immediate attention
                {
                    stockOutRiskStatus = "Immediate Attention";
                }
                else if (stockOutRiskIndicator > 10) // Hypothetical threshold for being cautious
                {
                    stockOutRiskStatus = "Caution";
                }
            }
            else
            {
                stockOutRiskStatus = "No Items";
            }

            // Create JSON object
            var result = new
            {
                TotalNumberOfItems = totalNumberOfItems,
                ItemsBelowMinThreshold = itemsBelowMinThreshold,
                StockOutRiskIndicator = stockOutRiskIndicator,
                StockOutRiskStatus = stockOutRiskStatus // Word indicating the status
            };

            return Ok(result);
        }

        [HttpGet("AverageInventoryTurnover")]
        public async Task<ActionResult<object>> GetAverageInventoryTurnover()
        {
            DateTime oneMonthAgo = DateTime.Now.AddDays(-30); // 30 days ago from today

            // Fetch all items from the database
            List<Item> allItems = await _context.Items.ToListAsync();
            decimal averageInventoryValue = 0;

            // Calculate the average inventory value based on the current cost price per piece
            if (allItems.Count > 0)
            {
                foreach (var item in allItems)
                {
                    decimal costPricePerPiece = item.PricePerPiece ?? 0; // Use 0 if CostPricePerPiece is null
                    averageInventoryValue += item.UnitsInInventory * costPricePerPiece;
                }
                averageInventoryValue /= allItems.Count;
            }

            // Fetch all sales from the database for the last 30 days
            List<Sale> recentSales = await _context.Sales.Where(s => s.SaleDate >= oneMonthAgo).ToListAsync();
            decimal totalSalesValue = 0;

            // Calculate the total sales value for the last 30 days
            foreach (var sale in recentSales)
            {
                totalSalesValue += sale.Total;
            }

            // Calculate the Average Inventory Turnover Ratio
            decimal averageInventoryTurnoverRatio = 0;
            if (averageInventoryValue != 0)
            {
                averageInventoryTurnoverRatio = totalSalesValue / averageInventoryValue;
            }

            // Create JSON object
            var result = new
            {
                AverageInventoryValue = averageInventoryValue,
                TotalSalesValue = totalSalesValue,
                AverageInventoryTurnoverRatio = averageInventoryTurnoverRatio
            };

            return Ok(result);
        }



        [HttpGet("TotalValueAndSales")]
        public async Task<ActionResult<object>> GetTotalValueAndSales()
        {
            // Fetch all items from the database
            List<Item> allItems = await _context.Items.ToListAsync();
            decimal totalInventoryValue = 0;

            // Calculate the total value of the inventory
            foreach (var item in allItems)
            {
                decimal pricePerUnit = item.PricePerUnit ?? 0; // Use 0 if PricePerUnit is null
                totalInventoryValue += item.UnitsInInventory * pricePerUnit;
            }

            // Fetch all sales from the database
            List<Sale> allSales = await _context.Sales.Include(s => s.ProductsInSale).ThenInclude(p => p.ComponentsInSale).ToListAsync();
            decimal totalSalesValue = 0;

            // Calculate the total value of sales
            foreach (var sale in allSales)
            {
                totalSalesValue += sale.Total;
            }

            // Determine if sales are enough to cover the cost of goods sold
            bool areSalesEnough = totalSalesValue >= totalInventoryValue;

            // Create JSON object
            var result = new
            {
                TotalInventoryValue = totalInventoryValue,
                TotalSalesValue = totalSalesValue,
                AreSalesEnough = areSalesEnough
            };

            return Ok(result);
        }

        private async Task<decimal> FetchUnitsSoldLast30Days(int itemId)
        {
            DateTime date30DaysAgo = DateTime.Now.AddDays(-30);

            var unitsSold = await _context.Sales
                .Where(x => x.SaleDate >= date30DaysAgo)
                .SelectMany(x => x.ProductsInSale)
                .SelectMany(x => x.ComponentsInSale)
                .Where(x => x.ItemId == itemId)
                .SumAsync(x => x.AmountUsed);

            return unitsSold;
        }


        private double CalculateStockToSalesRatio(int beginningOfMonthInventory, int salesForTheMonth)
        {
            if (salesForTheMonth == 0) return 0; // Prevent division by zero
            return (double)beginningOfMonthInventory / salesForTheMonth;
        }


        private double CalculateGMROII(decimal grossProfit, decimal averageInventoryCost)
        {
            if (averageInventoryCost == 0) return 0; // Prevent division by zero
            return (double)(grossProfit / averageInventoryCost);
        }


        private double CalculateInventoryTurnoverRate(int totalUnitsSold, int averageInventory)
        {
            if (averageInventory == 0) return 0; // Prevent division by zero
            return (double)totalUnitsSold / averageInventory;
        }


        [HttpGet("stock-levels")]
        public IActionResult GetItemsByStockLevels([FromQuery] string level)
        {
            IQueryable<Item> query = _context.Items;

            switch (level?.ToLower())
            {
                case "low":
                    query = query.Where(i => i.UnitsInInventory <= i.MinimumThreshold);
                    break;
                case "medium":
                    query = query.Where(i => i.UnitsInInventory > i.MinimumThreshold && i.UnitsInInventory < i.MaximumThreshold);
                    break;
                case "high":
                    query = query.Where(i => i.UnitsInInventory >= i.MaximumThreshold);
                    break;
                default:
                    return BadRequest("Invalid level parameter. Accepted values are: low, medium, high.");
            }

            var items = query.Select(i => new
            {
                i.ItemId,
                i.ItemName,
                i.UnitsInInventory,
                i.MinimumThreshold,
                i.MaximumThreshold,
                StockLevel = level
            })
            .ToList();

            return Ok(items);
        }

        [HttpGet("items-expiring-soon")]
        public IActionResult GetItemsExpiringSoon([FromQuery] int days)
        {
            if (days <= 0)
            {
                return BadRequest("Please provide a valid number of days.");
            }

            var targetExpiryDate = DateTime.Now.AddDays(days);

            var items = _context.Items
                                .Where(i => i.SellByDate.HasValue && i.SellByDate.Value.Date <= targetExpiryDate.Date)
                                .Select(i => new
                                {
                                    i.ItemId,
                                    i.ItemName,
                                    SellByDate = i.SellByDate.Value,
                                    i.UnitsInInventory,
                                    i.MinimumThreshold,
                                    i.MaximumThreshold
                                })
                                .ToList();

            return Ok(items);
        }



        private async Task<List<LinkedProductData>> FetchLinkedProducts(int itemId)
        {
            // Fetch the list of ProductToItem links for the given item
            var linkedProducts = await _context.ProductItems
                                               .Where(pti => pti.ItemId == itemId)
                                               .Include(pti => pti.Product)
                                               .ToListAsync();

            var result = new List<LinkedProductData>();

            // Loop through each linked product and calculate its profit
            foreach (var linkedProduct in linkedProducts)
            {
                var product = linkedProduct.Product;

                // Fetch all sales for this product
                var sales = await _context.Sales
                                          .Include(s => s.ProductsInSale)
                                          .Where(s => s.ProductsInSale.Any(p => p.ProductId == product.ProductId))
                                          .ToListAsync();

                // Calculate profit for this product
                decimal productProfit = (decimal)sales.Sum(s => s.ProductsInSale
                                                        .Where(p => p.ProductId == product.ProductId)
                                                        .Sum(p => p.ProductSellingPrice * p.Quantity) -
                                                        s.ProductsInSale
                                                        .Where(p => p.ProductId == product.ProductId)
                                                        .Sum(p => product.ProductCostPrice * p.Quantity));

                result.Add(new LinkedProductData
                {
                    ProductId = product.ProductId,
                    Name = product.ProductName,
                    Profit = productProfit
                });
            }

            return result;
        }

        public class LinkedProductData
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal Profit { get; set; }
        }

        private async Task<ItemExpirationData> FetchItemExpirationData(int itemId)
        {
            // Fetch expiration data from the database
            var expirationData = await _context.ItemExpirationData.FindAsync(itemId);

            // If data doesn't exist, create a new entry
            if (expirationData == null)
            {
                expirationData = new ItemExpirationData
                {
                    ItemId = itemId,
                    TimesExpired = 0,
                    LastExpirationDate = null
                };
                _context.ItemExpirationData.Add(expirationData);
                await _context.SaveChangesAsync();
            }

            return expirationData;
        }

        private async Task<List<LinkedItemData>> FetchOtherLinkedItemsData(int itemId)
        {
            // Fetch the product linked to this item
            var linkedProduct = await _context.ProductItems
                                              .Where(pti => pti.ItemId == itemId)
                                              .Select(pti => pti.Product)
                                              .FirstOrDefaultAsync();

            if (linkedProduct == null)
            {
                return new List<LinkedItemData>(); // No linked product found
            }

            // Fetch all items linked to this product
            var linkedItems = await _context.ProductItems
                                             .Where(pti => pti.ProductId == linkedProduct.ProductId)
                                             .Select(pti => pti.Item)
                                             .ToListAsync();

            // Fetch sales data where the linked product has been sold
            var productSalesData = await _context.Sales
                                                 .Where(s => s.ProductsInSale.Any(pis => pis.ProductId == linkedProduct.ProductId))
                                                 .SelectMany(s => s.ProductsInSale)
                                                 .Where(pis => pis.ProductId == linkedProduct.ProductId)
                                                 .Include(pis => pis.ComponentsInSale) // Include ComponentsInSale for later use
                                                 .ToListAsync();


            // Calculate the total profit of the linked product
            decimal totalProductProfit = 0m;
            foreach (var item in linkedItems)
            {
                decimal unitsFeaturedInSales = productSalesData.Sum(pis => pis.ComponentsInSale.Count(cis => cis.ItemId == item.ItemId));

                decimal sellingPrice = item.PricePerUnit ?? 0;
                decimal costPrice = linkedProduct.ProductCostPrice ?? 0;

                decimal itemProfit = (sellingPrice - costPrice) * unitsFeaturedInSales;
                totalProductProfit += itemProfit;
            }

            // Prepare the result
            var result = new List<LinkedItemData>();
            foreach (var item in linkedItems)
            {
                decimal unitsFeaturedInSales = productSalesData.Sum(pis => pis.ComponentsInSale.Count(cis => cis.ItemId == item.ItemId));

                decimal sellingPrice = item.PricePerUnit ?? 0;
                decimal costPrice = linkedProduct.ProductCostPrice ?? 0;

                decimal itemProfit = (sellingPrice - costPrice) * unitsFeaturedInSales;
                decimal contributionPercentage = totalProductProfit != 0 ? (itemProfit / totalProductProfit) * 100 : 0;

                result.Add(new LinkedItemData
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ContributionPercentage = contributionPercentage
                });
            }

            return result;
        }

        public class LinkedItemData
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public decimal ContributionPercentage { get; set; }
        }

        [HttpGet("CalculateItemSalesAndProfit/{itemId}/{timeframe}")]
        public async Task<IActionResult> CalculateItemSalesAndProfit(int itemId, string timeframe)
        {
            DateTime startDate, endDate;
            if (!TryParseTimeFrame(timeframe, out startDate, out endDate))
            {
                return BadRequest("Invalid timeframe specified. Use 'thisweek', 'lastweek', or 'thismonth'.");
            }

            decimal totalRevenue = await CalculateTotalItemRevenue(itemId, startDate, endDate);

            // Destructure the tuple into separate variables
            (decimal TotalProfit, decimal TotalCost) profitAndCost = await CalculateTotalItemProfit(itemId, startDate, endDate);


            return Ok(new
            {
                TotalRevenue = totalRevenue,
                TotalProfit = profitAndCost.TotalProfit,
                TotalCost = profitAndCost.TotalCost,
                Timeframe = timeframe,
                StartDate = startDate,
                EndDate = endDate
            });
        }

        private bool TryParseTimeFrame(string timeframe, out DateTime startDate, out DateTime endDate)
        {
            switch (timeframe.ToLower())
            {
                case "thisweek":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    endDate = DateTime.Today.AddDays(6 - (int)DateTime.Today.DayOfWeek);
                    return true;
                case "lastweek":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
                    endDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 1);
                    return true;
                case "thismonth":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    return true;
                default:
                    startDate = endDate = DateTime.MinValue;
                    return false;
            }
        }


        private async Task<List<Sale>> GetSalesByItemIdAsync(int itemId)
        {
            List<Sale> salesWithItem = await _context.Sales
                .Include(s => s.ProductsInSale)
                    .ThenInclude(p => p.ComponentsInSale)
                .Where(s => s.ProductsInSale.Any(p => p.ComponentsInSale.Any(c => c.ItemId == itemId)))
                .ToListAsync();
            return salesWithItem;
        }

        [HttpGet("CalculateItemSalesRevenue/{itemId}")]

        private async Task<decimal> CalculateTotalItemRevenue(int itemId, DateTime startDate, DateTime endDate)
        {
            decimal totalRevenue = 0M;

            // Fetch the item details from the database
            Item item = await _context.Items
                                      .Where(i => i.ItemId == itemId)
                                      .SingleOrDefaultAsync();

            if (item == null)
            {
                return 0M; // Item not found, returning zero revenue
            }

            decimal pricePerPiece = item.PricePerPiece.Value;

            // Fetch all the sales that include this item
            List<Sale> salesWithItem = await GetSalesByItemIdAsync(itemId, startDate, endDate);

            // Iterate through each sale
            foreach (var sale in salesWithItem)
            {
                // Iterate through each product in the sale
                foreach (var productInSale in sale.ProductsInSale)
                {
                    // Sum up the total cost of all components used in the product
                    decimal totalProductCost = productInSale.ComponentsInSale.Sum(c => c.AmountUsed * pricePerPiece);

                    // Calculate the total cost of our item used in the product
                    decimal itemCost = productInSale.ComponentsInSale
                                        .Where(c => c.ItemId == itemId)
                                        .Sum(c => c.AmountUsed * pricePerPiece);

                    // Calculate item's proportion in the total product cost
                    decimal itemProportion = (totalProductCost != 0) ? itemCost / totalProductCost : 0;

                    // Calculate revenue generated by the item from this product in the sale
                    decimal itemRevenue = itemProportion * productInSale.ProductSellingPrice * productInSale.Quantity;

                    // Add this to the total
                    totalRevenue += itemRevenue;
                }
            }

            return totalRevenue;
        }


        [HttpGet("CalculateItemProfitRevenue/{itemId}")]

        private async Task<(decimal TotalProfit, decimal TotalCost)> CalculateTotalItemProfit(int itemId, DateTime startDate, DateTime endDate)
        {
            decimal totalRevenue = 0M;
            decimal totalCost = 0M;

            // Fetch the item details from the database
            Item item = await _context.Items
                                      .Where(i => i.ItemId == itemId)
                                      .SingleOrDefaultAsync();

            if (item == null)
            {
                return (0M, 0M); // Item not found, returning zero profit and zero cost
            }

            decimal pricePerPiece = item.PricePerPiece.Value;

            // Fetch all the sales that include this item
            List<Sale> salesWithItem = await GetSalesByItemIdAsync(itemId, startDate, endDate);

            // Iterate through each sale
            foreach (var sale in salesWithItem)
            {
                // Iterate through each product in the sale
                foreach (var productInSale in sale.ProductsInSale)
                {
                    // Calculate the total cost of all components used in the product
                    decimal totalProductCost = productInSale.ComponentsInSale.Sum(c => c.AmountUsed * pricePerPiece);

                    // Calculate the total cost of our item used in the product
                    decimal itemCost = productInSale.ComponentsInSale
                                        .Where(c => c.ItemId == itemId)
                                        .Sum(c => c.AmountUsed * pricePerPiece);

                    // Calculate item's proportion in the total product cost
                    decimal itemProportion = (totalProductCost != 0) ? itemCost / totalProductCost : 0;

                    // Calculate revenue generated by the item from this product in the sale
                    decimal itemRevenue = itemProportion * productInSale.ProductSellingPrice * productInSale.Quantity;

                    // Add this to the total revenue
                    totalRevenue += itemRevenue;

                    // Add the item's cost to the total cost
                    totalCost += itemCost * productInSale.Quantity;
                }
            }

            // Calculate the profit
            decimal totalProfit = totalRevenue - totalCost;

            return (totalProfit, totalCost);
        }

        [HttpGet("GetLinkedProducts/{itemId}")]
        public async Task<List<LinkedProductDto>> GetLinkedProductsAsync(int itemId)
        {
            List<LinkedProductDto> linkedProducts = new List<LinkedProductDto>();

            // Fetch all product-to-item links that include the specified item
            var linksForItem = await _context.ProductItems
                                              .Include(l => l.Product)
                                              .Where(l => l.ItemId == itemId)
                                              .ToListAsync();

            // Loop through each product the item is linked to
            foreach (var link in linksForItem)
            {
                int productId = link.ProductId;
                string productName = link.Product?.ProductName;

                // Create and populate the DTO
                LinkedProductDto linkedProduct = new LinkedProductDto
                {
                    ProductId = productId,
                    ProductName = productName
                };

                // Add to the list
                linkedProducts.Add(linkedProduct);
            }

            return linkedProducts;
        }

        [HttpGet("CalculateItemUsageInLinkedProducts/{itemId}")]
        public async Task<IActionResult> CalculateItemUsageInLinkedProducts(int itemId)
        {
            var linkedProducts = await GetLinkedProductsAsync(itemId);
            List<ItemUsageInProductDto> itemUsages = new List<ItemUsageInProductDto>();

            foreach (var linkedProduct in linkedProducts)
            {
                var productSales = await _context.Sales
                    .Include(s => s.ProductsInSale)
                        .ThenInclude(p => p.ComponentsInSale)
                    .Where(s => s.ProductsInSale.Any(p => p.ProductId == linkedProduct.ProductId))
                    .ToListAsync();

                decimal totalItemUsageInProduct = 0;
                decimal totalProductComponents = 0;

                foreach (var sale in productSales)
                {
                    foreach (var productInSale in sale.ProductsInSale)
                    {
                        // Calculate the total usage of all items in this product
                        totalProductComponents += productInSale.ComponentsInSale.Sum(c => c.AmountUsed);

                        // Calculate the total usage of our item in this product
                        totalItemUsageInProduct += productInSale.ComponentsInSale
                                            .Where(c => c.ItemId == itemId)
                                            .Sum(c => c.AmountUsed);
                    }
                }

                // Calculate item's proportion in the product
                decimal itemUsageProportion = (totalProductComponents != 0) ? (totalItemUsageInProduct / totalProductComponents) * 100 : 0;

                // Populate the DTO
                ItemUsageInProductDto itemUsage = new ItemUsageInProductDto
                {
                    ProductId = linkedProduct.ProductId,
                    ProductName = linkedProduct.ProductName,
                    ItemUsageProportion = itemUsageProportion
                };

                itemUsages.Add(itemUsage);
            }

            return Ok(new { ItemUsages = itemUsages });
        }

        public class ItemUsageInProductDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal ItemUsageProportion { get; set; }
        }


        [HttpGet("EstimateReorderDateAndQuantity/{itemId}")]
        public async Task<IActionResult> EstimateReorderDateAndQuantity(int itemId)
        {
            // Fetch the item details
            Item item = await _context.Items
                                      .Where(i => i.ItemId == itemId)
                                      .SingleOrDefaultAsync();

            if (item == null)
            {
                return NotFound("Item not found.");
            }

            List<Sale> salesWithItem = await GetSalesByItemIdAsync(itemId);

            if (salesWithItem.Count == 0)
            {
                return NotFound("No sales data available for this item.");
            }

            // Calculate average daily sales for the item in the last 30 days
            DateTime oneMonthAgo = DateTime.Today.AddDays(-30);
            decimal totalAmountSoldLastMonth = salesWithItem.Where(s => s.SaleDate >= oneMonthAgo)
                                                            .Sum(s => s.ProductsInSale.Sum(p => p.ComponentsInSale.Where(c => c.ItemId == itemId).Sum(c => c.AmountUsed)));

            decimal averageDailyAmountSold = totalAmountSoldLastMonth / 30;


            // Calculate days until item reaches minimum threshold
            decimal remainingUnits = item.UnitsInInventory * (item.PiecesPerUnit ?? 1);
            decimal daysUntilMinThreshold = (remainingUnits - item.MinimumThreshold) / averageDailyAmountSold;

            // Estimate reorder date
            DateTime estimatedReorderDate = DateTime.Today.AddDays((double)daysUntilMinThreshold);

            // Calculate the estimated amount to order
            int estimatedAmountToOrder = (int)Math.Ceiling(30 * averageDailyAmountSold);

            // Determine item health based on days until minimum threshold
            string itemHealth;
            if (daysUntilMinThreshold > 15)
            {
                itemHealth = "Good";
            }
            else if (daysUntilMinThreshold > 7)
            {
                itemHealth = "Average";
            }
            else
            {
                itemHealth = "Bad";
            }

            return Ok(new
            {
                EstimatedReorderDate = estimatedReorderDate.ToString("d MMMM yyyy"),
                EstimatedAmountToOrder = estimatedAmountToOrder,
                ItemHealth = itemHealth
            });
        }


        // Also update your GetSalesByItemIdAsync to take startDate and endDate
        private async Task<List<Sale>> GetSalesByItemIdAsync(int itemId, DateTime startDate, DateTime endDate)
        {
            List<Sale> salesWithItem = await _context.Sales
                .Include(s => s.ProductsInSale)
                .ThenInclude(p => p.ComponentsInSale)
                .Where(s => s.ProductsInSale.Any(p => p.ComponentsInSale.Any(c => c.ItemId == itemId)))
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();
            return salesWithItem;
        }


        /// <summary>
        /// Meaasure how Much Of the Item Is left in the inventory, the Rate It Is being used at and When The Next Reorder Should Be made, use the miniunm Threshold data for this
        /// </summary>

        // DTO class
        public class LinkedProductDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
        }

    }
}
