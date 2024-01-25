using InventoryAPI.Controllers;
using InventoryAPI.Data;
using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using InventoryAPI.Model.SalesModel;
using InventoryAPI.Model.SupplierModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

public class StatsEngine
{
    private readonly DatabaseContext _context;

    public StatsEngine(DatabaseContext context)
    {
        _context = context;
    }

    //Get Sales Data
    public JsonResult TotalSalesRevenue(DateTime toDate)
    {
        DateTime currentDate = DateTime.Now; // Get the current date without time
        toDate = toDate.Date; // Remove time from the toDate parameter

        // Validate the toDate parameter
        if (toDate > currentDate)
        {
            return new JsonResult(new { error = "The given date cannot be in the future." });
        }

        // Fetch sales data and group by date
        var salesData = _context.Sales
                                .Where(s => s.SaleDate >= toDate && s.SaleDate <= currentDate)
                                .GroupBy(s => s.SaleDate)
                                .Select(g => new { SaleDate = g.Key.ToString("yyyy-MM-dd"), Total = g.Sum(s => (decimal?)s.Total) ?? 0M })
                                .ToList();

        return new JsonResult(salesData);
    }

    public string TotalSalesRevenueWithProfitLast30Days()
    {
        DateTime currentDate = DateTime.Now; // Get the current date and time
        DateTime toDate = currentDate.AddDays(-30); // Calculate 30 days ago

        // Fetch total sales revenue for the period
        decimal totalSalesRevenue = _context.Sales
                                    .Where(s => s.SaleDate >= toDate && s.SaleDate <= currentDate)
                                    .Sum(s => s.Total);

        // Initialize total cost of goods sold (COGS)
        decimal totalCOGS = 0M;

        // Fetch sales within the past 30 days
        var recentSales = _context.Sales
                                  .Include(s => s.ProductsInSale)
                                  .ThenInclude(p => p.Product)
                                  .Include(s => s.ProductsInSale)
                                  .ThenInclude(p => p.ComponentsInSale)
                                  .ThenInclude(c => c.Item)
                                  .Where(s => s.SaleDate >= toDate && s.SaleDate <= currentDate)
                                  .ToList();

        // Calculate total COGS
        foreach (var sale in recentSales)
        {
            foreach (var productInSale in sale.ProductsInSale)
            {
                totalCOGS += (decimal)(productInSale.Product.ProductCostPrice) * productInSale.Quantity; // Removed null-conditional as you can ensure this through your model constraints
                foreach (var componentInSale in productInSale.ComponentsInSale)
                {
                    totalCOGS += (decimal) (componentInSale.Item.PricePerUnit) * componentInSale.AmountUsed; // Removed null-conditional as you can ensure this through your model constraints
                }
            }
        }

        // Calculate profit. Removed the supplier costs from this calculation as per your requirement.
        decimal profit = totalSalesRevenue - totalCOGS;

        // Format the string for display
        string formattedString = string.Format(CultureInfo.InvariantCulture,
                                               "Total Sales = {0:N2}\nTotal Cost Of Goods = {1:N2}\nProfit = {2:N2}",
                                               totalSalesRevenue,
                                               totalCOGS,
                                               profit);

        return formattedString;
    }
    //Get COGS Cost Of Goods Sold
    public decimal PredictProfitFromStockOnHand()
    {
        // Sum of (SellingPrice - ProductCostPrice) * UnitsInInventory for all products
        var potentialProfit = _context.Products
                                      .Sum(p => (p.ProductSellingPrice - p.ProductCostPrice) * p.UnitsInInventory);
        return (decimal)potentialProfit;
    }


    public string CheckOrdersStatus()
    {
        DateTime currentDate = DateTime.Now;

        // Count orders that have arrived
        int arrivedOrders = _context.SupplierOrders
                                     .Where(o => o.Status == SupplierOrder.StatusOfOrder.Arrived)
                                     .Count();

        // Count orders that are past their due date
        int overdueOrders = _context.SupplierOrders
                                     .Where(o => o.Status != SupplierOrder.StatusOfOrder.Arrived &&
                                                 o.ExpectedDeliveryDate < currentDate)
                                     .Count();

        // Create a narrative
        string narrative = $"As of today, {arrivedOrders} supplier orders have arrived. " +
                           $"There are {overdueOrders} orders that are past their due date.";

        return narrative;
    }


    public int GetTotalNumberOfProducts()
    {
        return _context.Products.Count();
    }

    public decimal GetTotalWorthOfProducts()
    {
        // Sum of SellingPrice * UnitsInInventory for all products
        var totalWorth = _context.Products
                                 .Sum(p => p.ProductSellingPrice * p.UnitsInInventory);
        return (decimal)totalWorth;
    }

    public List<Product> GetProductsExpiringSoon(int daysBeforeExpiry = 30)
    {
        DateTime targetDate = DateTime.Now.AddDays(daysBeforeExpiry);
        return _context.Products
                       .Where(p => p.SellByDate <= targetDate)
                       .ToList();
    }


    public class PredictedStockDTO
    {
        public int Id { get; set; }  // Generic ID, can be either ProductId or ItemId
        public string Name { get; set; }  // ProductName or ItemName
        public decimal Price { get; set; }
        public decimal PredictedQuantity { get; set; }
        public string Type { get; set; }  // "Product" or "Item"
    }



    //this Function Needs to get all the sales and count how much was made and minus it from the total cost of all the order to the supplier made and the cost of those orders
    public decimal CalculateProfit(DateTime startDate, DateTime endDate)
    {
        // Fetch total sales for the period
        decimal totalSales = _context.Sales
                                     .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                                     .Sum(s => s.Total);

        // Fetch total supplier order costs for the period
        decimal totalSupplierCosts = _context.SupplierOrders
                                             .Where(so => so.OrderDate >= startDate && so.OrderDate <= endDate)
                                             .Sum(so => so.TotalCostOfOrder);

        // Calculate profit
        decimal profit = totalSales - totalSupplierCosts;

        return profit;
    }



    public List<Product> GetProductsRunningLow()
    {
        return _context.Products.Where(p => p.UnitsInInventory <= p.MinimumThreshold).ToList();
    }

    public List<Item> GetItemsRunningLow()
    {
        return _context.Items.Where(i => i.UnitsInInventory <= i.MinimumThreshold).ToList();
    }


    public List<Sale> GetSalesInDateRange(DateTime startDate, DateTime endDate)
    {
        try
        {
            return _context.Sales
                          .Include(s => s.ProductsInSale)
                          .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                          .ToList();
        }
        catch (Exception ex)
        {
            // Log the exception for debugging or monitoring purposes
            // For example: 
            // Logger.LogError($"Error fetching sales between {startDate} and {endDate}: {ex.Message}");

            // You can either return an empty list or rethrow the exception based on your requirements
            // Here, I'm returning an empty list to signify no results due to an error
            return new List<Sale>();
        }
    }


    public List<ProductSalesDTO> GetBestPerformingProducts(DateTime startDate, DateTime endDate, int topN = 5)
    {
        var productSales = from sale in _context.Sales
                           where sale.SaleDate >= startDate && sale.SaleDate <= endDate
                           from productInSale in sale.ProductsInSale
                           group productInSale by new { productInSale.ProductId, productInSale.Product.ProductName } into productGroup
                           select new ProductSalesDTO
                           {
                               ProductId = productGroup.Key.ProductId,
                               ProductName = productGroup.Key.ProductName,
                               TotalSold = productGroup.Sum(p => p.Quantity)
                           };

        return productSales.OrderByDescending(p => p.TotalSold).Take(topN).ToList();
    }


    public class ProductSalesDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalSold { get; set; }
    }


    public class CategorySalesDTO
    {
        public string CategoryName { get; set; }
        public decimal TotalRevenue { get; set; }
        public double SalesPercentage { get; set; }
    }

    // This function will get all the sales and count how much was made and subtract it from 
    // the total cost of all the orders to the supplier made and the cost of those orders
    public decimal CalculateGrossProfit()
    {
        // Fetch total sales for all time
        decimal totalSales = _context.Sales.Sum(s => s.Total);

        // Fetch total supplier order costs for all time
        decimal totalSupplierCosts = _context.SupplierOrders.Sum(so => so.TotalCostOfOrder);

        // Calculate gross profit
        decimal grossProfit = totalSales - totalSupplierCosts;

        return grossProfit;
    }


    // This function will get all the sales and sum up the total revenue
    public decimal CalculateEntireSalesRevenue()
    {
        // Fetch total sales revenue for all time
        decimal totalSalesRevenue = _context.Sales.Sum(s => s.Total);
        return totalSalesRevenue;
    }



    private decimal CalculateCOGS()
    {
        // Calculate the total cost for Products
        decimal totalProductCost = _context.Products.Sum(p => p.ProductCostPrice.HasValue ? p.ProductCostPrice.Value * p.UnitsInInventory : 0);

        // Calculate the total cost for Items
        decimal totalItemCost = _context.Items.Sum(i => i.PricePerUnit.HasValue ? i.PricePerUnit.Value * i.UnitsInInventory : 0);

        // Calculate the total COGS
        decimal totalCOGS = totalProductCost + totalItemCost;

        return totalCOGS;
    }


    //Heres A Stores Narrative to use in the Stats Page Simple Overview Of the store
    public JsonResult GenerateSimplifiedStoreNarrative()
    {
        Dictionary<string, string> narrative = new Dictionary<string, string>();

        // Evaluate Gross Profit
        decimal grossProfit = CalculateGrossProfit();
        decimal totalRevenue = CalculateEntireSalesRevenue();
        decimal profitMargin = (totalRevenue != 0) ? (grossProfit / totalRevenue) * 100 : 0;
        string grossProfitHealth = profitMargin >= 20 ? "Excellent" : profitMargin >= 10 ? "Fair" : "Poor";
        string roundedProfitMargin = profitMargin.ToString("0.00");
        narrative["GrossProfitSummary"] = $"Your profit margin is {grossProfitHealth}. The profit rate stands at {roundedProfitMargin}%.";

        // Evaluate Total Revenue
        decimal cogs = CalculateCOGS();
        decimal netRevenue = totalRevenue - cogs;
        string totalRevenueHealth = (netRevenue > 0) ? "Positive" : (netRevenue == 0 ? "Neutral" : "Negative");
        string netRevenueStatus = (netRevenue >= cogs) ? "covered the COGS" : "not covered the COGS";
        decimal coveragePercentage = (cogs != 0) ? (netRevenue / cogs) * 100 : 0;
        narrative["TotalRevenueSummary"] = $"Your net revenue appears {totalRevenueHealth}. You have {netRevenueStatus} by {coveragePercentage:C2}%.";

        // Evaluate Most and Least Profitable Products
        // Evaluate Best and Least Selling Products based on Sales Performance
        var bestSellingProducts = GetSalesPerformance(5, true);
        string bestSellingProductNames = string.Join(", ", bestSellingProducts.Select(p => p.ProductName));

        var leastSellingProducts = GetSalesPerformance(5, false);
        string leastSellingProductNames = string.Join(", ", leastSellingProducts.Select(p => p.ProductName));

        narrative["BestSellingProducts"] = $"Your best-selling products are: {bestSellingProductNames}.";
        narrative["LeastSellingProducts"] = $"Your least selling products are: {leastSellingProductNames}.";


        // Overall Health
        int goodMetrics = 0;
        goodMetrics += grossProfitHealth == "Excellent" ? 1 : 0;
        goodMetrics += totalRevenueHealth == "Positive" ? 1 : 0;
        goodMetrics += bestSellingProducts.Count > 0 ? 1 : 0;


        string overallHealth = (goodMetrics >= 2) ? "Excellent" : (goodMetrics == 1 ? "Fair" : "Poor");
        string recommendation = "";

        if (overallHealth == "Excellent")
        {
            recommendation = "Your store is doing extremely well! Consider scaling up.";
        }
        else if (overallHealth == "Fair")
        {
            recommendation = "Your store is doing okay, but there's room for improvement. Perhaps review your least profitable products.";
        }
        else
        {
            recommendation = "Your store needs attention, especially in profit margins and revenue. Consider revising your business strategy.";
        }

        narrative["OverallHealthSummary"] = $"Overall, your store's health appears to be {overallHealth}. {recommendation}";

        return new JsonResult(narrative);
    }

    //Get The Products Sales Performance Over A given Time Frame
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

    public class ExpiringAndSavingResult
    {
        public List<Product> ExpiringProducts { get; set; }
        public List<Item> ExpiringItems { get; set; }
        public string SavingMessage { get; set; }
    }

    public ExpiringAndSavingResult GetProductsAndItemsExpiringInNext7Days()
    {
        ExpiringAndSavingResult result = new ExpiringAndSavingResult();

        DateTime targetDate = DateTime.Now.AddDays(7); // Calculate the date 7 days from now

        // Fetch products expiring soon
        var productsExpiringSoon = _context.Products
                                           .Where(p => p.ExpiryDate != null && p.ExpiryDate <= targetDate)
                                           .ToList();

        // Fetch items expiring soon
        var itemsExpiringSoon = _context.Items
                                        .Where(i => i.SellByDate != null && i.SellByDate <= targetDate)
                                        .ToList();

        // Calculate the potential saving
        decimal potentialSavingFromProducts = productsExpiringSoon.Sum(p => p.ProductSellingPrice.HasValue ? p.ProductSellingPrice.Value : 0);
        decimal potentialSavingFromItems = itemsExpiringSoon.Sum(i => i.PricePerUnit.HasValue ? i.PricePerUnit.Value : 0);
        decimal totalPotentialSaving = potentialSavingFromProducts + potentialSavingFromItems;

        int totalExpiringItems = productsExpiringSoon.Count + itemsExpiringSoon.Count;

        // Create the saving message
        string savingMessage = $"Reduce Price of these {totalExpiringItems} expiring products to save {totalPotentialSaving:C2}. Place them on sale now!";

        // Combine both lists and the saving message
        result.ExpiringProducts = productsExpiringSoon;
        result.ExpiringItems = itemsExpiringSoon;
        result.SavingMessage = savingMessage;

        return result;
    }




}
