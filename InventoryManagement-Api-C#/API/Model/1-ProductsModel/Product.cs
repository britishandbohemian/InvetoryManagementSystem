using InventoryAPI.Model.CategoriesModel;
using InventoryAPI.Model.LinkModels;
using InventoryAPI.Model.SupplierModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace InventoryAPI.Model.ProductsModel
{
    // This Model Keeps Track Of A Product In the Inventory 
    public class Product
    {
        // Unique identifier for each product
        public int ProductId { get; set; }

        // Name of the product, required and limited to 50 characters
        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        // Foreign key for the category this product belongs to
        public int CategoryId { get; set; }
        // Navigation property for the category
        public virtual Category Category { get; set; }

        // Current number of units in inventory
        [DefaultValue(0)]
        public int UnitsInInventory { get; set; }

        // Markup percentage on the cost price
        [Range(0, 100)]
        public decimal MarkupPercentage { get; set; }
//Keep Tabs On When the product will Expire
        public DateTimeOffset ExpiryDate { get; set; }

        // Selling price of the product, calculated from cost price
        public decimal? ProductSellingPrice { get; set; }

        // Cost price of the product
        public decimal? ProductCostPrice { get; set; }
        public decimal? PreviousProductCostPrice { get; set; }

        // Minimum number of units that should be in inventory
        [Required]
        [DefaultValue(0)]
        public int MinimumThreshold { get; set; }

        // Maximum number of units that should be in inventory
        [Required]
        [DefaultValue(0)]
        public int MaximumThreshold { get; set; }

        // Current status of the product (Active, Inactive, etc.)
        [DefaultValue(ProductStatus.Active)]
        public ProductStatus Status { get; set; }

        public enum ProductStatus
        {
            Active,
            Inactive,
            SoldOut,
            Expired,
            Onsale
        }

        // Units of measurement for this product (Liter, Kilogram, etc.)
        public ProductUnitsOfMeasurement UnitOfMeasurement { get; set; }

        public enum ProductUnitsOfMeasurement
        {
            Liter,
            Kilogram,
            Count,
            Gram
        }


        // The date by which the product should be sold
        public DateTimeOffset? SellByDate { get; set; }

        // The date when the product was last sold
        public DateTime? LastSoldDate { get; set; }

        // Foreign key for the supplier of this product
        [ForeignKey("SupplierId")]
        public int? SupplierId { get; set; }
        // Navigation property for the supplier
        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }

        // The date when this product entry was created
        public DateTime? DateCreated { get; set; }

        // Navigation property for products ordered
        public virtual ICollection<OrderProduct> OrderedProducts { get; set; }
        // Navigation property for items linked to this product
        public ICollection<ProductToItem> LinkedItems { get; set; }
        // Navigation property for expiration events related to this product
        public virtual ICollection<ProductExpirationEvent>? ExpirationEvents { get; set; }
        //Product Images
        public virtual string ImageUrl { get; set; }

        public virtual string Description { get; set; } 
        public virtual string Qoute { get; set; } 
        public virtual int Rating { get; set; } = 0;

        public virtual ICollection<Review> Reviews { get; set; } 

    }
}
