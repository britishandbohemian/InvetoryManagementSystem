namespace InventoryAPI.Controllers
{
    internal class SalesPerformanceDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public int UnitsInInventory { get; set; }
        public int MinimumThreshold { get; set; }
        public int MaximumThreshold { get; set; }
    }
}