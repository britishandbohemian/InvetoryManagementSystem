namespace InventoryAPI.DTO.InventoryDto
{
    public class SaleDTO
    {
        public int SaleId { get; set; }

        public List<ProductForSaleDTO> ProductsForSale { get; set; }

        public decimal Total { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public DateTime SaleDate { get; set; }

        public int UserId { get; set; }

        public string AdditionalInfo { get; set; }

        public SaleDTOPaymentMethod PaymentType { get; set; }

        public enum SaleDTOPaymentMethod
        {
            Cash,
            CreditCard,
            MobilePayment,
        }

        public class ProductForSaleDTO
        {


            public int ProductId { get; set; }

            public string ProductName { get; set; }  // We can include this for better clarity in the DTO

            public int Quantity { get; set; }

            public decimal ProductSellingPrice { get; set; }

            public List<ComponentForSaleDTO> ComponentsInSale { get; set; }  // List of components used in the sale
        }

        public class ComponentForSaleDTO
        {


            public int ItemId { get; set; }

            public string ItemName { get; set; }  // We can include this for better clarity in the DTO

            public decimal AmountUsed { get; set; }
        }
    }
}
