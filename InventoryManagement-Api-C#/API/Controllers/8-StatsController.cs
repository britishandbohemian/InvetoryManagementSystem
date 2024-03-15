using InventoryAPI.Data;
using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly StatsEngine _statsEngine;

        public StatsController(DatabaseContext context)
        {
            _context = context;
            _statsEngine = new StatsEngine(_context);  // pass the context to the StatsEngine
        }

        // GET: api/stats/totalrevenue?toDate=2023-10-07
        [HttpGet("totalrevenue")]
        public IActionResult GetTotalSalesRevenue([FromQuery] DateTime toDate)
        {
            // Validate the toDate parameter
            if (toDate.Date > DateTime.Now.Date)
            {
                return BadRequest("The given date cannot be in the future.");
            }

            var revenue = _statsEngine.TotalSalesRevenue(toDate);
            return Ok(revenue);
        }

        [HttpGet("ExpiringAndPotentialSavings")]
        public IActionResult GetExpiringProductsAndPotentialSavings()
        {
            var result = _statsEngine.GetProductsAndItemsExpiringInNext7Days();
            return Ok(result);
        }

        [HttpGet("totalrevenuePastThirtyDays")]
        public IActionResult GetTotalSalesThrityDays()
        {
            var revenue = _statsEngine.TotalSalesRevenueWithProfitLast30Days();
            return Ok(revenue);
        }


        [HttpGet("check-orders-status")]
        public IActionResult CheckOrdersStatusFunc()
        {
            string orderStatusNarrative = _statsEngine.CheckOrdersStatus();
            return Ok(new { Narrative = orderStatusNarrative });
        }

        [HttpGet("potential-profit")]
        public IActionResult GetPotentialProfitFromStockOnHand()
        {
            decimal potentialProfit = _statsEngine.PredictProfitFromStockOnHand();
            return Ok(new { PotentialProfit = potentialProfit });
        }

        //Refine
        [HttpGet("entiresalesrevenue")]
        public IActionResult GetEntireSalesRevenue()
        {
            decimal totalSalesRevenue = _statsEngine.CalculateEntireSalesRevenue();
            return Ok(new { TotalSalesRevenue = totalSalesRevenue });
        }


        [HttpGet("recommend-for-sale")]
        public ActionResult<string> GetRecommendedProductForSale()
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Initialize variables to hold the recommended product and the reason
            Product recommendedProduct = null;
            string reason = string.Empty;

            // Identify a product that is about to expire in 15 days and is not already on sale (Status != 4)
            DateTime targetDate = currentDate.AddDays(15);
            recommendedProduct = _context.Products
                                         .FirstOrDefault(p => p.SellByDate != null && p.SellByDate.Value.Date <= targetDate && p.Status != Product.ProductStatus.Onsale);

            if (recommendedProduct != null)
            {
                reason = "About to expire.";
            }
            else
            {
                // Identify a low turnover product that is not already on sale (Status != 4)
                recommendedProduct = _context.Products
                                             .FirstOrDefault(p => p.UnitsInInventory > 10 && p.LastSoldDate < DateTime.Now.AddMonths(-3) && p.Status != Product.ProductStatus.Onsale);

                if (recommendedProduct != null)
                {
                    reason = "Low turnover.";
                }
                else
                {
                    // Identify an overstocked product that is not already on sale (Status != 4)
                    recommendedProduct = _context.Products
                                                 .FirstOrDefault(p => p.UnitsInInventory > p.MaximumThreshold && p.Status != Product.ProductStatus.Onsale);

                    if (recommendedProduct != null)
                    {
                        reason = "Overstocked.";
                    }
                }
            }

            // If a product is found, convert to JSON or any other format you prefer
            if (recommendedProduct != null)
            {
                var simplifiedProduct = new
                {
                    Name = recommendedProduct.ProductName,
                    ProductId = recommendedProduct.ProductId,
                    LastSoldDate = recommendedProduct.LastSoldDate?.ToString("yyyy-MM-dd"),
                    Reason = reason
                };

                string jsonResult = JsonConvert.SerializeObject(simplifiedProduct);
                return Ok($"{jsonResult}");
            }
            else
            {
                return Ok("No product recommended for sale.");
            }
        }


        [HttpGet("GetGrossProfitForPast30Days")]
        public ActionResult<string> GetGrossProfitForPast30Days()
        {
            DateTime currentDate = DateTime.Now.Date; // Get the current date without time
            DateTime fromDate = currentDate.AddDays(-30); // Get the date 30 days ago

            // Calculate Total Sales Revenue for the past 30 days
            decimal totalSalesRevenue = _context.Sales
                                                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= currentDate)
                                                .Sum(s => s.Total);

            // Initialize Total Cost Of Goods Sold (COGS)
            decimal totalCostOfGoods = 0;

            // Fetch all sales in the past 30 days
            var salesInPast30Days = _context.Sales
                                            .Include(s => s.ProductsInSale)
                                            .ThenInclude(p => p.ComponentsInSale)
                                            .Where(s => s.SaleDate >= fromDate && s.SaleDate <= currentDate)
                                            .ToList();

            // Calculate the total cost of goods for sold products and components
            foreach (var sale in salesInPast30Days)
            {
                foreach (var productInSale in sale.ProductsInSale)
                {
                    // Check if there are components in the sale for this product
                    if (productInSale.ComponentsInSale == null || !productInSale.ComponentsInSale.Any())
                    {
                        continue;  // Skip to the next productInSale if no components are present
                    }

                    // Add the cost of the product sold to totalCostOfGoods
                    totalCostOfGoods += productInSale.ProductSellingPrice * productInSale.Quantity;

                    foreach (var componentInSale in productInSale.ComponentsInSale)
                    {
                        // Null check for componentInSale and componentInSale.Item
                        if (componentInSale != null && componentInSale.Item != null)
                        {
                            // Add the cost of the component used in the sale to totalCostOfGoods
                            totalCostOfGoods += componentInSale.AmountUsed * (componentInSale.Item.PricePerUnit ?? 0);
                        }
                    }
                }
            }

            // Calculate Gross Profit for the past 30 days
            decimal grossProfit = totalSalesRevenue - totalCostOfGoods;

            return Ok($"The Gross Profit for the past 30 days (from {fromDate.ToString("yyyy-MM-dd")} to {currentDate.ToString("yyyy-MM-dd")}) is R{grossProfit:F2}.");
        }


        public class PredictedProductStockDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            // Add any other product properties you need
            public decimal PredictedQuantity { get; set; }
        }

        [HttpGet("combined-stats")]
        public IActionResult GetCombinedStats([FromQuery] int daysBeforeExpiry = 10, [FromQuery] int topN = 20)
        {
            // Fetch expiring products count and details
            var expiringDate = DateTime.Now.AddDays(daysBeforeExpiry);
            var expiringProducts = _context.Products
                .Where(p => p.SellByDate <= expiringDate)
                .Select(p => new { p.ProductId, p.ProductName, p.SellByDate })
                .ToList();

            var expiringProductCount = expiringProducts.Count;

            // Fetch best-performing products
            var startDate = DateTime.Now.Date.AddDays(-30);
            var endDate = DateTime.Now.Date;

            var bestPerformingProducts = _context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .SelectMany(s => s.ProductsInSale)
                .GroupBy(p => p.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSales = g.Sum(p => p.Quantity) })
                .OrderByDescending(g => g.TotalSales)
                .Take(topN)
                .ToList();

            // Combine both into a JSON object
            var result = new
            {
                ExpiringProductCount = expiringProductCount,
                ExpiringProducts = expiringProducts,
                BestPerformingProducts = bestPerformingProducts
            };

            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("calculate-all-products-cog")]
        public ActionResult<IEnumerable<object>> CalculateAllProductsCOG()
        {
            var products = _context.Products.ToList();
            var cogList = products.Select(p => new
            {
                ProductId = p.ProductId,
                COG = p.UnitsInInventory * (p.ProductCostPrice ?? 0)
            }).ToList();

            return Ok(cogList);
        }


        [HttpGet("calculate-all-items-cog")]
        public ActionResult<IEnumerable<object>> CalculateAllItemsCOG()
        {
            var items = _context.Items.ToList();
            var cogList = items.Select(i => new
            {
                ItemId = i.ItemId,
                COG = i.UnitsInInventory * (i.PricePerUnit ?? 0)
            }).ToList();

            return Ok(cogList);
        }

        [HttpGet("LowStockSummary")]
        public async Task<ActionResult<object>> GetLowStockSummary()
        {
            // Fetch low stock products
            var lowStockProducts = await _context.Products
                .Where(p => p.UnitsInInventory <= p.MinimumThreshold)
                .Select(p => new
                {
                    Type = "Product",
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Action = p.LinkedItems.Any() ? "Remake" : "Reorder",
                    Description = p.LinkedItems.Any() ? "Product with linked items" : ""
                })
                .ToListAsync();

            // Fetch low stock items
            var lowStockItems = await _context.Items
                .Where(i => i.UnitsInInventory <= i.MinimumThreshold)
                .Select(i => new
                {
                    Type = "Item",
                    Id = i.ItemId,
                    Name = i.ItemName,
                    Action = "Reorder",
                    Description = ""
                })
                .ToListAsync();

            // Create a list to hold the summary
            List<object> lowStockSummary = new List<object>();

            // Check if any products or items are low
            bool noLowStockProducts = !lowStockProducts.Any();
            bool noLowStockItems = !lowStockItems.Any();

            // If both are "No stock low", return "Good"
            if (noLowStockProducts && noLowStockItems)
            {
                return Ok("Good");
            }

            // Add products if any
            if (!noLowStockProducts)
            {
                lowStockSummary.AddRange(lowStockProducts);
            }

            // Add items if any
            if (!noLowStockItems)
            {
                lowStockSummary.AddRange(lowStockItems);
            }

            return Ok(lowStockSummary);
        }


        public static class COGCalculator
        {
            // Calculate COG for Product
            public static decimal CalculateProductCOG(Product product)
            {
                if (product == null || product.UnitsInInventory <= 0 || product.ProductCostPrice == null)
                {
                    return 0;
                }

                return product.UnitsInInventory * product.ProductCostPrice.Value;
            }

            // Calculate COG for Item
            public static decimal CalculateItemCOG(Item item)
            {
                if (item == null || item.UnitsInInventory <= 0 || item.PricePerUnit == null)
                {
                    return 0;
                }

                return item.UnitsInInventory * item.PricePerUnit.Value;
            }
        }


    }
}
