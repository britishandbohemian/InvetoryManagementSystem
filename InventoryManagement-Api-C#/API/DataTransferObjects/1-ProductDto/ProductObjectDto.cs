using InventoryAPI.DataTransferObjects.ItemDto;
using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{

    //The Basics of what each product has to have in order to create one
    public class BaseProductDTO
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string ProductName { get; set; }

        //Can Be made up of prices of components or the cost price from the supplier
        //Or Not know Before it is ordered
        public decimal ProductCostPrice { get; set; }

        [Required]
        //Create a range that forces the markup to be above 100 atleast to ensure a profit
        public decimal MarkupPercentage { get; set; }

        //This should be calculated using the markup amount
        //This Variable will also change once the product is order as that cost price will also determine what the selling price is but intially the selling price will be calcualted when the product is made this ensure that the store does not run at a loss due to input errors the selling privce will always be above the cost price
        public decimal ProductSellingPrice { get; set; }


        //Not Know At Creation
        public int? SupplierId { get; set; }


        public Product.ProductUnitsOfMeasurement UnitOfMeasurement { get; set; } // Added unit of measurement

        [Required]
        public int MinimumThreshold { get; set; } // Added

        [Required]
        public int MaximumThreshold { get; set; } // Added

        //not Know at Creation
        public DateTimeOffset? SellByDate { get; set; } // Added


        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public Product.ProductStatus Status { get; set; } = Product.ProductStatus.Active; // Added

        [Required]
        public virtual string Description { get; set; }
        [Required]
        public virtual string Qoute { get; set; }
        [Required]
        public IFormFile ImageFile { get; set; }

    }

    public class CreateProductDTO : BaseProductDTO
    {
        //Create a product, and if it has items then let them come with quantities
        public List<ItemWithQuantity> ItemQuantities { get; set; }
    }

    public class ItemWithQuantity
    {
        public int ItemId { get; set; }
        public decimal QuantityUsed { get; set; }
    }

    public class UpdateProductDTO : BaseProductDTO
    {
        public List<ItemWithQuantity>? ItemQuantities { get; set; }
    }

    public class ReadItemWithQuantityDTO
    {
        public BaseItemDTO Item { get; set; }
        public decimal QuantityUsed { get; set; }
    }

    public class ReadProductDTO : BaseProductDTO
    {
        public string SupplierName { get; set; }
        public int ProductId { get; set; }

        public string ImageUrl { get; set; }
        public int UnitsInInventory { get; set; }
       public string CategoryName { get; set; }
        public List<ReadItemWithQuantityDTO> LinkedItems { get; set; } = new List<ReadItemWithQuantityDTO>();
    }

}
