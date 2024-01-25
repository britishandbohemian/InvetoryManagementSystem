using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using InventoryAPI.Model.UserModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.SalesModel
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        public virtual ICollection<ProductInSale> ProductsInSale { get; set; }

        public decimal Total { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public DateTime SaleDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string? AdditionalInfo { get; set; }

        public PaymentMethod PaymentType { get; set; }

        public enum PaymentMethod
        {
            Cash,
            CreditCard,
            MobilePayment,
        }

        public class ProductInSale
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key]
            public int ProductInSaleId { get; set; }

            [ForeignKey("Product")]
            public int ProductId { get; set; }

            public virtual Product Product { get; set; }

            public int Quantity { get; set; }

            public decimal ProductSellingPrice { get; set; }

            public virtual ICollection<ComponentInSale> ComponentsInSale { get; set; }  // List of components used in the sale

            [ForeignKey("Sale")]  // Adding ForeignKey attribute for Sale
            public int SaleId { get; set; }  // Adding SaleId

            [JsonIgnore]
            public Sale Sale { get; set; }

        }


        public class ComponentInSale
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key]
            public int ComponentInSaleId { get; set; }

            [ForeignKey("ProductInSale")]
            public int ProductInSaleId { get; set; }

            [Required]
            [JsonIgnore]
            public virtual ProductInSale ProductInSale { get; set; }

            [ForeignKey("Item")]
            public int ItemId { get; set; }

            [Required]
            public virtual Item Item { get; set; }

            public decimal AmountUsed { get; set; }
        }
    }
}
