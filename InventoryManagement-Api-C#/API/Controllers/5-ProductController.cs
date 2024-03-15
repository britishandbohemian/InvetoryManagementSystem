using InventoryAPI.Data;
using InventoryAPI.DataTransferObjects.ItemDto;
using InventoryAPI.DTOs;
using InventoryAPI.Model.LinkModels;
using InventoryAPI.Model.ProductsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventoryAPI.Model.ProductsModel.Product;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;


namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        //This is use to acces my db
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(DatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            //This is Used to know where my hosting enviroment Dir is
            _webHostEnvironment = webHostEnvironment;
        }



        //Base Function to create a product
        [HttpPost("CreateProductsWithoutComponents")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] BaseProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request if the model state is invalid
            }

            string savedImagePath = await SaveImage(dto.ImageFile);
            List<Product> createdProducts = new List<Product>();

            Product product = new Product
            {
                CategoryId = dto.CategoryId,
                ProductName = dto.ProductName,
                ProductCostPrice = dto.ProductCostPrice,
                MarkupPercentage = dto.MarkupPercentage,
                ProductSellingPrice = (((dto.MarkupPercentage / 100) + 1) * dto.ProductCostPrice),
                UnitOfMeasurement = dto.UnitOfMeasurement,
                MinimumThreshold = dto.MinimumThreshold,
                MaximumThreshold = dto.MaximumThreshold,
                DateCreated = DateTime.UtcNow, // Set the current date as the creation date
                Status = dto.Status,
                SupplierId = dto.SupplierId,  // Set SupplierId, it can be null
                Description = dto.Description,
                Qoute = dto.Qoute,
                ImageUrl = savedImagePath, // Save the path of the saved image
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                productId = product.ProductId // Return the ID of the created product
            });
        }


        // Function to handle image saving
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(imageDirectory, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var relativeImagePath = Path.Combine("images", imageName);
                return relativeImagePath;
            }

            return null;
        }

        // Function to create products with components
        [HttpPost("CreateProductsWithComponents")]
        public async Task<ActionResult<IEnumerable<Product>>> CreateProductsWithComponents([FromForm] List<CreateProductDTO> dtos)
        {
            List<Product> createdProducts = new List<Product>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var dto in dtos)
                    {
                        if (dto.ItemQuantities == null || !dto.ItemQuantities.Any())
                        {
                            return BadRequest("At least one component with its quantity must be provided.");
                        }

                        var componentPrices = _context.Items
                            .Where(i => dto.ItemQuantities.Select(item => item.ItemId).Contains(i.ItemId))
                            .ToDictionary(i => i.ItemId, i => i.PricePerUnit ?? 0M);

                        decimal totalComponentCost = dto.ItemQuantities
                            .Sum(item => componentPrices.ContainsKey(item.ItemId) ? componentPrices[item.ItemId] * item.QuantityUsed : 0);

                        // Calculate the selling price based on the total component cost and markup percentage
                        decimal markupValue = totalComponentCost * (dto.MarkupPercentage / 100);
                        string savedImagePath = await SaveImage(dto.ImageFile);

                        Product product = new Product
                        {
                            CategoryId = dto.CategoryId,
                            ProductName = dto.ProductName,
                            ProductCostPrice = totalComponentCost,
                            MarkupPercentage = dto.MarkupPercentage,
                            ProductSellingPrice = totalComponentCost + markupValue,
                            UnitOfMeasurement = dto.UnitOfMeasurement,
                            MinimumThreshold = dto.MinimumThreshold,
                            MaximumThreshold = dto.MaximumThreshold,
                            DateCreated = DateTime.UtcNow,
                            Status = dto.Status,
                            Description = dto.Description,
                            Qoute = dto.Qoute,
                            ImageUrl = savedImagePath
                        };

                        _context.Products.Add(product);
                        await _context.SaveChangesAsync();  // Save the product to ensure it gets an ID.
                        createdProducts.Add(product);

                        foreach (var itemWithQuantity in dto.ItemQuantities)
                        {
                            _context.ProductItems.Add(new ProductToItem
                            {
                                ProductId = product.ProductId,
                                ItemId = itemWithQuantity.ItemId,
                                AmountUsed = itemWithQuantity.QuantityUsed
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the exception (ex) here if you have a logging mechanism
                    return StatusCode(500, "An error occurred while creating the product with components. Please try again.");
                }
            }

            return Ok(new
            {
                status = "success",
                productIds = createdProducts.Select(p => p.ProductId)
            });
        }


        //Get a Product Using its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadProductDTO>> GetProduct(int id)
        {
            // Fetching the product
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Fetching links directly from the ProductToItem table
            var linkedItems = await _context.ProductItems
                                            .Where(pi => pi.ProductId == id)
                                            .Include(pi => pi.Item)
                                            .Select(pi => new ReadItemWithQuantityDTO
                                            {
                                                Item = new BaseItemDTO
                                                {
                                                    ItemName = pi.Item.ItemName,
                                                    UnitOfMeasurement = pi.Item.UnitOfMeasurement,
                                                    PricePerUnit = pi.Item.PricePerUnit,
                                                    PiecesPerUnit = pi.Item.PiecesPerUnit,
                                                    SellByDate = pi.Item.SellByDate,
                                                    MinimumThreshold = pi.Item.MinimumThreshold,
                                                    MaximumThreshold = pi.Item.MaximumThreshold,
                                                    SupplierId = pi.Item.SupplierId ?? 0,
                                                    Status = pi.Item.Status
                                                },
                                                QuantityUsed = pi.AmountUsed
                                            })
                                            .ToListAsync();

            var dto = new ReadProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Description = product.Description,
                Qoute = product.Qoute,
                ProductCostPrice = product.ProductCostPrice ?? 0,
                MarkupPercentage = product.MarkupPercentage,
                UnitOfMeasurement = product.UnitOfMeasurement,
                MinimumThreshold = product.MinimumThreshold,
                MaximumThreshold = product.MaximumThreshold,
                SellByDate = product.SellByDate,
                Status = product.Status,
                LinkedItems = linkedItems
            };

            return Ok(dto);
        }

        //Update a product Information
        [HttpPut("UpdateProduct/{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
        {
            var product = await _context.Products
                                        .Include(p => p.LinkedItems) // Include linked items
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Update product fields
            product.ProductName = dto.ProductName;
            product.CategoryId = dto.CategoryId;
            product.PreviousProductCostPrice = product.ProductCostPrice;
            product.ProductCostPrice = dto.ProductCostPrice;
            product.MarkupPercentage = dto.MarkupPercentage;
            product.MinimumThreshold = dto.MinimumThreshold;
            product.MaximumThreshold = dto.MaximumThreshold;
            product.Status = dto.Status;

            // Update QuantityUsed for each linked item
            foreach (var itemWithQuantity in dto.ItemQuantities)
            {
                var linkedItem = product.LinkedItems.FirstOrDefault(li => li.ItemId == itemWithQuantity.ItemId);

                if (linkedItem != null)
                {
                    linkedItem.AmountUsed = itemWithQuantity.QuantityUsed;
                }
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //Read Product Information From Database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadProductDTO>>> GetAllProducts()
        {
            await CheckAndRecordExpirationEvents();

            var products = await _context.Products
                                         .Include(p => p.LinkedItems)
                                         .ThenInclude(pi => pi.Item)
                                         .Include(p => p.Category)  // Include the Category
                                         .Include(p => p.Supplier)  // Include the Supplier
                                         .ToListAsync();

            var productDtos = products.Select(product => new ReadProductDTO
            {
                ProductId = product.ProductId,
                SupplierId = product.SupplierId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName,  // Map Category's name
                ProductCostPrice = product.ProductCostPrice ?? 0,
                ProductSellingPrice = product.ProductSellingPrice ?? 0,
                MarkupPercentage = product.MarkupPercentage,
                UnitOfMeasurement = product.UnitOfMeasurement,
                UnitsInInventory = product.UnitsInInventory,
                MinimumThreshold = product.MinimumThreshold,
                MaximumThreshold = product.MaximumThreshold,
                SellByDate = product.SellByDate,
                Status = product.Status,
                Description = product.Description,
                Qoute = product.Qoute,
                ImageUrl = product.ImageUrl,  // Include ImageUrl
                SupplierName = product.Supplier?.SupplierName,  // Map Supplier's name
                LinkedItems = product.LinkedItems.Select(pi => new ReadItemWithQuantityDTO
                {
                    Item = new BaseItemDTO
                    {
                        ItemName = pi.Item.ItemName,
                        ItemId = pi.Item.ItemId,
                        UnitOfMeasurement = pi.Item.UnitOfMeasurement,
                        PricePerUnit = pi.Item.PricePerUnit,
                        PiecesPerUnit = pi.Item.PiecesPerUnit,
                        SellByDate = pi.Item.SellByDate,
                        MinimumThreshold = pi.Item.MinimumThreshold,
                        MaximumThreshold = pi.Item.MaximumThreshold,
                        SupplierId = pi.Item.SupplierId ?? 0,
                        Status = pi.Item.Status
                    },
                    QuantityUsed = pi.AmountUsed
                }).ToList()
            }).ToList();

            return Ok(productDtos);
        }








        //-----------------------------------------------------------------------------
        /* Other Helper Functions and methods to perform Analysis on the product Table*/

        [HttpPost("ReducePrice")]
        public async Task<ActionResult<Product>> ReducePrice(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Reduce the selling price by 5%
            product.ProductSellingPrice *= 0.95M;

            // Change the status to "Onsale"
            product.Status = Product.ProductStatus.Onsale;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(product);
        }


        [HttpPost("Dicontinue")]
        public async Task<ActionResult<Product>> Dicontinue(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }


            // Change the status to "Onsale"
            product.Status = Product.ProductStatus.Inactive;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(product);
        }


        // This could be a method in your API rather than a stored property



        //Get The Products Sales Performance
        private List<SalesPerformanceDto> GetSalesPerformance(int count, bool descending)
        {
            var salesPerformanceQuery = _context.Sales
                .SelectMany(Sale => Sale.ProductsInSale)  // Flatten the collections of ProductsInSale into a single sequence
                .GroupBy(pis => new
                {
                    pis.Product.ProductId,
                    pis.Product.ProductName,
                    pis.Product.UnitsInInventory,
                    pis.Product.MinimumThreshold,
                    pis.Product.MaximumThreshold
                })  // Group by ProductId, ProductName, UnitsInInventory, MinimumThreshold, and MaximumThreshold
                .Select(g => new SalesPerformanceDto  // Project into SalesPerformanceDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(pis => pis.Quantity),  // Sum the quantities
                    UnitsInInventory = g.Key.UnitsInInventory,
                    MinimumThreshold = g.Key.MinimumThreshold,
                    MaximumThreshold = g.Key.MaximumThreshold
                });

            if (descending)
            {
                salesPerformanceQuery = salesPerformanceQuery.OrderByDescending(p => p.TotalSold);
            }
            else
            {
                salesPerformanceQuery = salesPerformanceQuery.OrderBy(p => p.TotalSold);
            }

            return salesPerformanceQuery.Take(count).ToList();
        }

        //Get Products That Are going to Expire Within The next 7 Days
        [HttpGet("near-expiration")]
        public IActionResult GetProductsNearExpiration()
        {
            var thresholdDate = DateTime.Now.AddDays(7);

            var products = _context.Products
                .Where(p => p.SellByDate != null &&
                            p.SellByDate <= thresholdDate &&
                            p.Status != ProductStatus.Onsale)  // Exclude products that are already on sale
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.SellByDate,
                    p.UnitsInInventory,
                    p.MinimumThreshold,
                    p.MaximumThreshold,
                    p.Status  // Include the status in the output if needed
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("top-selling")]
        public IActionResult GetTopSellingProducts([FromQuery] int top)
        {
            if (top <= 0)
            {
                return BadRequest("The top parameter must be greater than 0.");
            }

            // Assuming GetSalesPerformance gives you a list of Product objects
            var topSelling = GetSalesPerformance(top, descending: true)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    SalesRevenue = CalculateSalesRevenueForProduct(p.ProductId)  // Call a method to calculate the sales revenue for this product
                                                                                 // Add other necessary fields
                })
                .ToList();

            return Ok(topSelling);
        }

        private decimal CalculateSalesRevenueForProduct(int productId)
        {
            // Use LINQ to sum up the Total field for all Sales where this product appears.
            // Assuming you have a DbContext named dbContext, and it has a DbSet<Sale> named Sales
            decimal totalRevenue = _context.Sales
                .Where(s => s.ProductsInSale.Any(p => p.ProductId == productId))
                .Sum(s => s.Total);

            return totalRevenue;
        }

        [HttpGet("least-selling")]
        public IActionResult GetLeastSellingProducts()
        {


            DateTime currentDate = DateTime.Now; // Get the current date
            DateTime fromDate = currentDate.AddDays(-30); // Calculate 30 days ago

            // Get the total sales for each product in the last 30 days
            var totalSalesPerProduct = _context.Sales
                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= currentDate)
                .SelectMany(s => s.ProductsInSale)
                .GroupBy(p => p.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSales = g.Sum(p => p.Quantity) })
                .ToList();

            // Get the list of products and their details
            var productList = _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.UnitsInInventory,
                    p.MinimumThreshold,
                    p.MaximumThreshold,
                    p.Status

                })
                .ToList();

            // Get least selling products by comparing with total sales
            var leastSelling = (from p in productList
                                join ts in totalSalesPerProduct on p.ProductId equals ts.ProductId into gj
                                from x in gj.DefaultIfEmpty()
                                where p.Status != ProductStatus.Onsale // Exclude products that are already on sale
                                orderby x?.TotalSales ?? 0
                                select new
                                {
                                    p.ProductId,
                                    p.ProductName,
                                    p.UnitsInInventory,
                                    p.MinimumThreshold,
                                    p.MaximumThreshold,
                                    p.Status,  // Include the Status
                                    TotalSales = x?.TotalSales ?? 0
                                })
                .Take(10)
                .ToList();


            return Ok(leastSelling);
        }

        [HttpGet("stock-levels")]
        public IActionResult GetProductsByStockLevels([FromQuery] string level)
        {
            IQueryable<Product> query = _context.Products;

            switch (level?.ToLower())
            {
                case "low":
                    query = query.Where(p => p.UnitsInInventory <= p.MinimumThreshold);
                    break;
                case "medium":
                    query = query.Where(p => p.UnitsInInventory > p.MinimumThreshold + 20 && p.UnitsInInventory < p.MaximumThreshold);
                    break;
                case "high":
                    query = query.Where(p => p.UnitsInInventory >= p.MaximumThreshold);
                    break;
                default:
                    return BadRequest("Invalid level parameter. Accepted values are: low, medium, high.");
            }

            var products = query.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.UnitsInInventory,
                p.MinimumThreshold,
                p.MaximumThreshold,
                StockLevel = level,
                HasItem = p.LinkedItems != null ? "Yes" : "No"  // Check if the product has an item
            })
            .ToList();

            return Ok(products);
        }


        // GET: api/Inventory/TotalProducts
        [HttpGet("TotalProducts")]
        public async Task<ActionResult<object>> GetTotalProducts()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalUnitsInInventory = await _context.Products.SumAsync(p => p.UnitsInInventory);
            var totalValueOfInventory = await _context.Products.SumAsync(p => p.UnitsInInventory * (p.ProductCostPrice ?? 0));

            var totalSellingValueOfInventory = await _context.Products.SumAsync(p => p.UnitsInInventory * (p.ProductSellingPrice ?? 0));
            var projectedProfit = totalSellingValueOfInventory - totalValueOfInventory;

            var result = new
            {
                TotalProducts = totalProducts,
                TotalUnitsInInventory = totalUnitsInInventory,
                TotalValueOfInventory = totalValueOfInventory,
                ProjectedProfit = projectedProfit
            };

            return Ok(result);
        }


        // GET: api/Inventory/OverstockedProducts
        [HttpGet("OverstockedProducts")]
        public async Task<ActionResult<int>> GetOverstockedProducts()
        {
            var overstockedProducts = await _context.Products
                .Where(p => p.UnitsInInventory >= p.MaximumThreshold)
                .CountAsync();
            return Ok(overstockedProducts);
        }


        //Link a Product To component When it is made
        [HttpPost("link")]
        public async Task<IActionResult> LinkProductComponent([FromBody] ProductToItem productComponent)
        {
            var product = await _context.Products.FindAsync(productComponent.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var component = await _context.Items.FindAsync(productComponent.ItemId);
            if (component == null)
            {
                return NotFound("Component not found");
            }

            var existingLink = await _context.ProductItems.FirstOrDefaultAsync(pc => pc.ProductId == productComponent.ProductId && pc.ItemId == productComponent.ItemId);
            if (existingLink != null)
            {
                return BadRequest("Link between this product and component already exists");
            }

            _context.ProductItems.Add(productComponent);
            await _context.SaveChangesAsync();

            return Ok("Link created successfully");
        }


        [HttpGet("low-stock-products")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            // Query for products running low on stock
            var lowStockProducts = await _context.Products
                .Where(p => p.UnitsInInventory <= p.MinimumThreshold)
                .Select(p => new LowStockProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitsInInventory = p.UnitsInInventory,
                    MinimumThreshold = p.MinimumThreshold,
                    RecommendedReorderAmount = CalculateRecommendedReorderAmount(p.MinimumThreshold, p.UnitsInInventory)
                })
                .ToListAsync();

            return Ok(lowStockProducts);
        }


        [HttpGet("GetSalesAndProfitOverDays")]
        public async Task<ActionResult<object>> GetSalesAndProfitOverDays([FromQuery] int productId, [FromQuery] int days)
        {
            DateTime startDate = DateTime.Now.AddDays(-days); // X days back from today

            // Profit for specific product
            var salesData = await _context.Sales
                .Where(s => s.SaleDate >= startDate && s.ProductsInSale.Any(p => p.ProductId == productId))
                .SelectMany(s => s.ProductsInSale.Where(p => p.ProductId == productId))
                .Select(p => new { p.Quantity, p.ProductSellingPrice, p.Product.ProductCostPrice })
                .ToListAsync();

            int totalUnitsSold = salesData.Sum(x => x.Quantity);
            decimal totalRevenue = salesData.Sum(x => x.Quantity * x.ProductSellingPrice);
            decimal totalCost = salesData.Sum(x => x.Quantity * (x.ProductCostPrice ?? 0));
            decimal totalProfit = totalRevenue - totalCost;

            // Overall store profit
            var allSalesData = await _context.Sales
                .Where(s => s.SaleDate >= startDate)
                .SelectMany(s => s.ProductsInSale)
                .Select(p => new { p.Quantity, p.ProductSellingPrice, p.Product.ProductCostPrice })
                .ToListAsync();

            decimal totalStoreRevenue = allSalesData.Sum(x => x.Quantity * x.ProductSellingPrice);
            decimal totalStoreCost = allSalesData.Sum(x => x.Quantity * (x.ProductCostPrice ?? 0));
            decimal totalStoreProfit = totalStoreRevenue - totalStoreCost;

            // Calculate percentage
            decimal percentageOfTotalProfit = 0;
            if (totalStoreProfit != 0)
            {
                percentageOfTotalProfit = (totalProfit / totalStoreProfit) * 100;
            }

            return Ok(new
            {
                TotalUnitsSold = totalUnitsSold,
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                PercentageOfTotalProfit = percentageOfTotalProfit
            });
        }



        //Information Given Back    
        //product information
        //Name Of Product
        //Profitability
        //I can get this data for this week, Last week, This month, All time
        //-----------
        //How Much Did It Make the store
        // Add this to your ProductController.cs
        [HttpGet("GetProductSalesStatistics")]
        public async Task<IActionResult> GetProductSalesStatisticsGet([FromQuery] int productId, [FromQuery] string timeFrame)
        {
            if (productId <= 0 || string.IsNullOrEmpty(timeFrame))
            {
                return BadRequest("Invalid parameters");
            }

            // Call the GetProductSalesStatistics method here (assuming it's implemented in the same class)
            var result = await GetProductSalesAndProfitStatistics(productId, timeFrame);

            return Ok(result);
        }

        // Define a method in your ProductController or a service class
        private async Task<object> GetProductSalesAndProfitStatistics(int productId, string timeFrame)
        {
            // Initialize DateTime variables for the time frame
            DateTime startDate, endDate;
            DateTime now = DateTime.Now;

            // Determine time frame
            switch (timeFrame)
            {
                case "thisWeek":
                    startDate = now.AddDays(-7);
                    endDate = now;
                    break;
                case "lastWeek":
                    startDate = now.AddDays(-14);
                    endDate = now.AddDays(-7);
                    break;
                case "thisMonth":
                    startDate = now.AddDays(-30);
                    endDate = now;
                    break;
                default:
                    return BadRequest("Invalid time frame");
            }

            // Fetch sales data for the product between startDate and endDate
            var salesData = await _context.Sales
                .Include(s => s.ProductsInSale)
                .Where(s => s.ProductsInSale.Any(p => p.ProductId == productId) && s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();

            // Fetch the product data to get its accurate ProductCostPrice
            var productData = await _context.Products
                .Where(p => p.ProductId == productId)
            .FirstOrDefaultAsync();



            // Initialize variables for total sales and total cost
            decimal totalSalesAmount = 0;
            decimal totalCostAmount = 0;

            foreach (var sale in salesData)
            {
                var productInSale = sale.ProductsInSale.FirstOrDefault(p => p.ProductId == productId);
                if (productInSale != null)
                {
                    // Total sales amount is the sum of (Selling Price * Quantity) for each sale
                    totalSalesAmount += productInSale.ProductSellingPrice * productInSale.Quantity;

                    // Total cost amount is the sum of (Cost Price * Quantity) for each sale
                    if (productData != null && productData.ProductCostPrice.HasValue)
                    {
                        totalCostAmount += productData.ProductCostPrice.Value * productInSale.Quantity;
                    }
                }
            }


            // Calculate the profit
            decimal profit = totalSalesAmount - totalCostAmount;




            var allSalesData = await _context.Sales
    .Include(s => s.ProductsInSale)
    .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
    .ToListAsync();

            // Calculate the total sales for the entire store
            decimal totalStoreSales = 0;
            foreach (var sale in allSalesData)
            {
                totalStoreSales += sale.Total;
            }

            // Calculate the percentage impact of the product on the store's sales
            decimal productImpactPercentage = 0;
            if (totalStoreSales != 0)
            {
                productImpactPercentage = (totalSalesAmount / totalStoreSales) * 100;
            }


            // Fetch the items linked to the product
            var linkedItems = await _context.ProductItems
                .Include(pti => pti.Item)
                .Where(pti => pti.ProductId == productId)
                .ToListAsync();

            // Initialize list to hold the supplies information
            var suppliesInfo = new List<object>();

            // Initialize a variable to hold the total amount used for all items
            decimal totalAmountUsed = linkedItems.Sum(li => li.AmountUsed);

            // Iterate over each linked item to gather information
            foreach (var linkedItem in linkedItems)
            {
                var item = linkedItem.Item;
                var itemName = item.ItemName;
                var itemId = item.ItemId;

                // Calculate the current health of the item (inventory level vs minimum threshold)
                // Calculate the current health of the item (inventory level vs maximum capacity)

                // Your existing code
                var currentHealth = Math.Min(100, (decimal)item.UnitsInInventory / (decimal)item.MaximumThreshold * 100);
                var impactOnProduct = Math.Min(100, (linkedItem.AmountUsed / totalAmountUsed) * 100);

                // Round to 2 decimal places
                var roundedCurrentHealth = Math.Round(currentHealth, 2);
                var roundedImpactOnProduct = Math.Round(impactOnProduct, 2);

                // Add this information to the list
                suppliesInfo.Add(new
                {
                    ItemId = itemId,
                    ItemName = itemName,
                    CurrentHealth = $"{roundedCurrentHealth}%", // as a percentage
                    ImpactOnProduct = $"{roundedImpactOnProduct}%"  // as a percentage
                });;

                // ... (return this information along with previously calculated sales and impact data)



                // Return the total sales and product impact percentage

            }
            return Ok(new { TotalSalesAmount = totalSalesAmount, TotalProfit = profit, ProductImpactPercentage = productImpactPercentage, SuppliesUsed = suppliesInfo });

        }

        //How Much Profit Did it make
        //What Are The Products Best Selling Dates Monday Tuesday Wednesday?
        //What Is the Products Imapact to the overall Store in a percentage within the given time frame


        //Supplies Information
        //List Of all the supplies used to make this product
        //Their Health Interms of the their stock levels
        //How Much in Percentage do they Make the Product so 25% of the product is made from this item

        ///Finally Decision
        ///Either Place on sale, Stop Selling, promote it
        /// <summary>
        /// 
        /// </summary>


        //Helper Functions
        //Make Not When It Expires
        private async Task CheckAndRecordExpirationEvents()
        {
            var products = await _context.Products.ToListAsync();
            foreach (var product in products)
            {
                if (product.SellByDate.HasValue && product.SellByDate.Value < DateTimeOffset.Now)
                {
                    var expirationEvent = new ProductExpirationEvent
                    {
                        ProductId = product.ProductId,
                        ExpirationDate = DateTimeOffset.Now,
                        UnitsInInventoryAtExpiration = product.UnitsInInventory
                    };

                    _context.ProductExpirationEvents.Add(expirationEvent);
                }
            }
            await _context.SaveChangesAsync();
        }

        // DTO for Low Stock Products
        public class LowStockProductDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int UnitsInInventory { get; set; }
            public int MinimumThreshold { get; set; }
            public int RecommendedReorderAmount { get; set; }
        }

        // Helper function to calculate recommended reorder amount
        private static int CalculateRecommendedReorderAmount(int minimumThreshold, int unitsInInventory)
        {
            // You can customize this logic as per your business needs
            return minimumThreshold * 2 - unitsInInventory;
        }



    }
}
