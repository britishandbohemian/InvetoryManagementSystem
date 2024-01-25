
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Model.CategoriesModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace InventoryAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        // Custom CategoryWithTrends object for API response
        public class CategoryWithTrends : Category
        {
            public Dictionary<string, decimal> SeasonalTrends { get; set; }
        }

        // Your API endpoint to get all categories with additional details
        [HttpGet("categories-with-details")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriesWithDetails()
        {
            var categories = await _context.Categories
                                           .Include(c => c.Products)  // Include related Products
                                           .Include(c => c.Items)     // Include related Components
                                           .ToListAsync();

            var categoriesWithDetails = categories.Select(category =>
            {
                decimal profitMargin = 0;
                Dictionary<string, decimal> seasonalTrends = null;

                try
                {
                    profitMargin = CalculateCategoryProfitMargin(category.CategoryId);
                    seasonalTrends = GetSeasonalTrendsForCategoryFunction(category.CategoryId);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                }

                return new
                {
                    category.CategoryId,
                    category.CategoryName,
                    category.CategoryDescription,
                    Products = category.Products,
                    Items = category.Items,
                    CategoryWorth = category.Products.Sum(p => p.ProductSellingPrice) + category.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory),
                    ProductsWorth = category.Products.Sum(p => p.ProductSellingPrice),
                    ItemsWorth = category.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory),
                    ProfitMargin = profitMargin,
                    SeasonalTrends = seasonalTrends
                };
            }).OrderByDescending(category => category.ProfitMargin).ToList();  // Sort by profit margin, highest to lowest

            return Ok(categoriesWithDetails);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryObjectDto addCategoryDto)
        {
            var category = new Category
            {
                CategoryName = addCategoryDto.CategoryName,
                CategoryDescription = addCategoryDto.CategoryDescription
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { status = "error", message = "Category not found" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { status = "success", message = "Category deleted successfully" });
        }


        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto updateCategoryDto)
        {
            if (id != updateCategoryDto.CategoryId)
            {
                return BadRequest();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = updateCategoryDto.CategoryName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CategoryExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }


        public class CategoryUpdateDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string CategoryDescription { get; set; }
        }


        [HttpGet("{id}/bestdays")]
        public async Task<ActionResult> GetCategoryBestDays(int id)
        {
            // Fetch the required data first
            var sales = await _context.Sales
                .Include(s => s.ProductsInSale)
                .ThenInclude(p => p.Product)
                .ToListAsync();

            // Now filter and group the data in memory
            var salesByDay = sales
                .Where(s => s.ProductsInSale.Any(p => p.Product.CategoryId == id))
                .GroupBy(s => s.SaleDate.DayOfWeek)
                .Select(g => new { DayOfWeek = g.Key, TotalSales = g.Sum(s => s.Total) })
                .OrderBy(g => g.DayOfWeek)
                .ToList();

            if (salesByDay.Count == 0)
            {
                return NotFound("No sales data for this category.");
            }

            var chartData = new
            {
                Labels = salesByDay.Select(s => s.DayOfWeek.ToString()).ToArray(),
                Data = salesByDay.Select(s => s.TotalSales).ToArray()
            };

            return Ok(chartData);
        }



        private Dictionary<string, decimal> GetSeasonalTrendsForCategoryFunction(int categoryId)
        {
            Dictionary<string, decimal> seasonalTrends = new Dictionary<string, decimal>();

            var sales = _context.Sales
                                .Include(s => s.ProductsInSale)
                                .ThenInclude(ps => ps.Product)
                                .ThenInclude(p => p.Category)
                                .Where(s => s.ProductsInSale.Any(ps => ps.Product.CategoryId == categoryId))
                                .ToList();

            if (!sales.Any())
            {
                seasonalTrends["NoSalesInfo"] = 0;
                return seasonalTrends;
            }

            var salesByMonth = sales.GroupBy(s => s.SaleDate.ToString("yyyy-MM"));

            foreach (var salesGroup in salesByMonth)
            {
                decimal monthlyRevenue = 0;
                decimal monthlyCost = 0;

                foreach (var sale in salesGroup)
                {
                    foreach (var productInSale in sale.ProductsInSale)
                    {
                        if (productInSale.Product.CategoryId != categoryId)
                        {
                            continue;
                        }

                        if (productInSale.Product.ProductSellingPrice.HasValue && productInSale.Product.ProductCostPrice.HasValue)
                        {
                            monthlyRevenue += productInSale.Product.ProductSellingPrice.Value * productInSale.Quantity;
                            monthlyCost += productInSale.Product.ProductCostPrice.Value * productInSale.Quantity;
                        }
                    }
                }

                DateTime firstSaleDate = salesGroup.First().SaleDate;
                string monthName = firstSaleDate.ToString("MMMM", CultureInfo.InvariantCulture);
                string yearAndMonthName = firstSaleDate.Year + "-" + monthName;

                decimal monthlyProfit = monthlyRevenue - monthlyCost;
                seasonalTrends[yearAndMonthName] = monthlyProfit;
            }

            return seasonalTrends;
        }

        private string GenerateSeasonalTrendsNarrative(int categoryId)
        {
            var seasonalTrends = GetSeasonalTrendsForCategoryFunction(categoryId);
            StringBuilder narrative = new StringBuilder();

            if (seasonalTrends.ContainsKey("NoSalesInfo"))
            {
                return "<p class='alert alert-warning'>There are no sales to analyze for this category.</p>";
            }

            // Identify the best and worst months based on the aggregated values (could be sales, profit, etc.)
            var bestMonth = seasonalTrends.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            var worstMonth = seasonalTrends.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

            narrative.AppendLine("<ul class='list-group mb-4'>");
            narrative.AppendLine($"  <li class='list-group-item list-group-item-success'>Best Performing Season: <strong>{bestMonth}</strong></li>");
            narrative.AppendLine($"  <li class='list-group-item list-group-item-danger'>Worst Performing Season: <strong>{worstMonth}</strong></li>");
            narrative.AppendLine("</ul>");

            return narrative.ToString();
        }

        private decimal CalculateCategoryProfitMargin(int categoryId)
        {
            var sales = _context.Sales
                                .Include(s => s.ProductsInSale)
                                    .ThenInclude(ps => ps.Product)
                                        .ThenInclude(p => p.Category)
                                .Include(s => s.ProductsInSale)
                                    .ThenInclude(ps => ps.ComponentsInSale)
                                        .ThenInclude(cs => cs.Item)
                                .Where(s => s.ProductsInSale.Any(ps => ps.Product.CategoryId == categoryId))
                                .ToList();

            if (!sales.Any())
            {
                throw new Exception("No sales found for this category.");
            }

            decimal totalRevenue = 0;
            decimal totalCost = 0;

            foreach (var sale in sales)
            {
                foreach (var productInSale in sale.ProductsInSale)
                {
                    if (productInSale.Product.CategoryId != categoryId)
                    {
                        continue;
                    }

                    if (productInSale.Product.ProductSellingPrice.HasValue)
                    {
                        totalRevenue += productInSale.Product.ProductSellingPrice.Value * productInSale.Quantity;
                    }

                    if (productInSale.Product.ProductCostPrice.HasValue)
                    {
                        totalCost += productInSale.Product.ProductCostPrice.Value * productInSale.Quantity;
                    }

                    foreach (var componentInSale in productInSale.ComponentsInSale)
                    {
                        if (componentInSale.Item.PricePerUnit.HasValue)
                        {
                            totalCost += componentInSale.AmountUsed * componentInSale.Item.PricePerUnit.Value;
                        }
                    }
                }
            }

            if (totalRevenue <= 0)
            {
                return 0;
            }
            decimal profitMargin = 0;
            if (totalRevenue != 0)  // Avoid division by zero
            {
                profitMargin = ((totalRevenue - totalCost) / totalRevenue) * 100;
            }
            return profitMargin;
        }
        private string BuildCategoryNarrative(int categoryId)
        {
            var category = _context.Categories
                                    .Include(c => c.Products)
                                    .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return JsonConvert.SerializeObject(new { error = "Category not found." });
            }

            var profitMargin = CalculateCategoryProfitMargin(categoryId);
            var roundedProfitMargin = Math.Round(profitMargin, 2);
            var seasonalTrendsNarrative = GenerateSeasonalTrendsNarrative(categoryId);
            var financials = CalculateCategoryFinancials(categoryId);

            StringBuilder narrative = new StringBuilder();
            narrative.AppendLine("<div class='container-fluid'>");  // Changed to container-fluid for full width
            narrative.AppendLine($"  <h1 class='display-4' style='font-size:2.5rem;font-weight:bold;width:100%;'>{category.CategoryName} Financial Summary</h1>");

            narrative.AppendLine("  <ul class='list-group mb-4'>");
            narrative.AppendLine($"    <li class='list-group-item'>Total Revenue: {financials.TotalRevenue:C2}</li>");
            narrative.AppendLine($"    <li class='list-group-item'>Total Cost: {financials.TotalCost:C2}</li>");

            // Style the Total Profit based on its value
            string profitClass = "list-group-item";
            if (financials.TotalProfit < 0)
            {
                profitClass = "list-group-item' style='background-color: #FF0000; color: white;";
            }
            else if (financials.TotalProfit >= 0 && financials.TotalProfit < 10000) // Replace 10000 with your own threshold for 'average'
            {
                profitClass = "list-group-item' style='background-color: #FFA500;"; // Orange for 'average'
            }
            else
            {
                profitClass = "list-group-item' style='background-color: #008000; color: white;"; // Green for 'good'
            }
            narrative.AppendLine($"    <li class='{profitClass}'><strong>Total Profit:</strong> {financials.TotalProfit:C2}</li>");

            // Style the Profit Margin based on its value
            string marginClass = "list-group-item";
            if (roundedProfitMargin < 0)
            {
                marginClass = "list-group-item' style='background-color: #FF0000; color: white;";
            }
            else if (roundedProfitMargin >= 0 && roundedProfitMargin < 10) // Replace 10 with your own threshold for 'average'
            {
                marginClass = "list-group-item' style='background-color: #FFA500;"; // Orange for 'average'
            }
            else
            {
                marginClass = "list-group-item' style='background-color: #008000; color: white;"; // Green for 'good'
            }
            narrative.AppendLine($"    <li class='{marginClass}'><strong>Profit Margin:</strong> {roundedProfitMargin}%</li>");
            narrative.AppendLine("  </ul>");


            // Seasonal Trends with Time Frame
            narrative.AppendLine("  <h2 class='display-5' style='font-size:2rem;font-weight:bold;width:100%;'>Seasonal Trends (Last 3 Months)</h2>");
            narrative.AppendLine($"  <p>{seasonalTrendsNarrative}</p>");



            // Actions for Improvement
            narrative.AppendLine("  <h2 class='display-5' style='font-size:2rem;font-weight:bold;width:100%;'>Actions for Improvement</h2>");
            narrative.AppendLine("  <div style='margin-top:1rem;'>");
            narrative.AppendLine($"    <button class='btn' onclick='promoteProduct({categoryId})'>Promote Products</button>");
            narrative.AppendLine($"    <button class='btn' onclick='initiateSale({categoryId})'>Initiate Sale On Products</button>");

            narrative.AppendLine("  </div>");
            narrative.AppendLine("</div>");

            return narrative.ToString();
        }

        public class CategoryFinancials
        {
            public decimal TotalRevenue { get; set; }
            public decimal TotalCost { get; set; }
            public decimal TotalProfit { get; set; }
        }

        private CategoryFinancials CalculateCategoryFinancials(int categoryId)
        {
            var sales = _context.Sales
                                .Include(s => s.ProductsInSale)
                                    .ThenInclude(ps => ps.Product)
                                        .ThenInclude(p => p.Category)
                                .Include(s => s.ProductsInSale)
                                    .ThenInclude(ps => ps.ComponentsInSale)
                                        .ThenInclude(cs => cs.Item)
                                .Where(s => s.ProductsInSale.Any(ps => ps.Product.CategoryId == categoryId))
                                .ToList();

            if (!sales.Any())
            {
                throw new Exception("No sales found for this category.");
            }

            decimal totalRevenue = 0;
            decimal totalCost = 0;

            foreach (var sale in sales)
            {
                foreach (var productInSale in sale.ProductsInSale)
                {
                    if (productInSale.Product.CategoryId != categoryId)
                    {
                        continue;
                    }

                    if (productInSale.Product.ProductSellingPrice.HasValue)
                    {
                        totalRevenue += productInSale.Product.ProductSellingPrice.Value * productInSale.Quantity;
                    }

                    if (productInSale.Product.ProductCostPrice.HasValue)
                    {
                        totalCost += productInSale.Product.ProductCostPrice.Value * productInSale.Quantity;
                    }

                    foreach (var componentInSale in productInSale.ComponentsInSale)
                    {
                        if (componentInSale.Item.PricePerUnit.HasValue)
                        {
                            totalCost += componentInSale.AmountUsed * componentInSale.Item.PricePerUnit.Value;
                        }
                    }
                }
            }

            return new CategoryFinancials
            {
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                TotalProfit = totalRevenue - totalCost
            };
        }



        private IQueryable<Category> FilterCategoriesByWorthLevel(IQueryable<Category> query, string level)
        {
            switch (level.ToLower())
            {
                case "low":
                    return query.Where(c =>
                        (c.Products.Sum(p => p.ProductSellingPrice) + c.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory)) < 1000);
                case "medium":
                    return query.Where(c =>
                        (c.Products.Sum(p => p.ProductSellingPrice) + c.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory)) >= 1000 &&
                        (c.Products.Sum(p => p.ProductSellingPrice) + c.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory)) < 10000);
                case "high":
                    return query.Where(c =>
                        (c.Products.Sum(p => p.ProductSellingPrice) + c.Items.Sum(i => i.PricePerUnit * i.UnitsInInventory)) >= 10000);
                default:
                    return query;
            }
        }


        [HttpGet("filter-by-worth-level")]
        public ActionResult<IEnumerable<Category>> FilterCategoriesByWorthLevelEndpoint(string level)
        {
            var filteredCategories = FilterCategoriesByWorthLevel(_context.Categories.AsQueryable(), level).ToList();
            return filteredCategories;
        }


        // GET: api/categories/5/metrics?timeFrame=ThisWeek
        [HttpGet("{id}/metrics")]
        public async Task<ActionResult> GetCategoryMetrics(int id, string timeFrame)
        {
            DateTime startDate, endDate;

            switch (timeFrame)
            {
                case "ThisWeek":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    endDate = startDate.AddDays(7);
                    break;
                case "LastWeek":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
                    endDate = startDate.AddDays(7);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
                default:
                    return BadRequest("Invalid time frame.");
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            var totalSales = CalculateTotalSales(category, startDate, endDate);
            var totalProfit = CalculateTotalProfit(category, startDate, endDate);
            var storeTotalWorth = await CalculateStoreTotalWorth(startDate, endDate);
            var categoryImpact = CalculateCategoryImpact(category, storeTotalWorth);
            var bestMonthAndSeason = await GetBestPerformingMonthAndSeason(id);

            return Ok(new
            {
                TotalSales = totalSales,
                TotalProfit = totalProfit,
                CategoryImpact = categoryImpact,
                BestMonthAndSeason = bestMonthAndSeason
            });
        }

        private decimal CalculateTotalSales(Category category, DateTime startDate, DateTime endDate)
        {
            // Add filter to only sum up sales within the time frame
            return category.Products?.Sum(p => p.ProductSellingPrice * p.UnitsInInventory) ?? 0;
        }

        private decimal CalculateTotalProfit(Category category, DateTime startDate, DateTime endDate)
        {
            // Add filter to only sum up profit within the time frame
            return category.Products?.Sum(p => (p.ProductSellingPrice - p.ProductCostPrice) * p.UnitsInInventory) ?? 0;
        }

        private async Task<decimal> CalculateStoreTotalWorth(DateTime startDate, DateTime endDate)
        {
            // Add filter to only sum up worth within the time frame
            return (decimal)await _context.Products.SumAsync(p => p.ProductSellingPrice * p.UnitsInInventory);
        }

        // ... (rest of the functions remain the same)

        private decimal CalculateCategoryImpact(Category category, decimal storeTotalWorth)
        {
            var categoryWorth = category.Products?.Sum(p => p.ProductSellingPrice * p.UnitsInInventory) ?? 0;
            return (categoryWorth / storeTotalWorth) * 100;
        }

        private async Task<string> GetBestPerformingMonthAndSeason(int categoryId)
        {
            var salesData = await _context.Sales
                .Include(s => s.ProductsInSale)
                .Where(s => s.ProductsInSale.Any(p => p.Product.CategoryId == categoryId))
                .GroupBy(s => s.SaleDate.Month)
                .Select(g => new { Month = g.Key, TotalSales = g.Sum(s => s.Total) })
                .OrderByDescending(g => g.TotalSales)
                .FirstOrDefaultAsync();

            if (salesData == null)
            {
                return "No sales data available";
            }

            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(salesData.Month);
            var season = GetSeason(salesData.Month);

            return $"Best performing month: {monthName}, known for: {season}";
        }

        private string GetSeason(int month)
        {
            switch (month)
            {
                case 12:
                case 1:
                case 2:
                    return "Winter";
                case 3:
                case 4:
                case 5:
                    return "Spring";
                case 6:
                case 7:
                case 8:
                    return "Summer";
                case 9:
                case 10:
                case 11:
                    return "Autumn";
                default:
                    return "Unknown";
            }
        }

    }
}
