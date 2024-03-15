using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.LinkModels
{
    // A Table used to link a product To a component
    public class ProductToItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LinkId { get; set; }

        [ForeignKey("ProductId")]
        //Id Of the product
        public int ProductId { get; set; }

        [JsonIgnore]
        public Product? Product { get; set; }

        [ForeignKey("ItemId")]

        //The Id of the component
        public int ItemId { get; set; }

        [JsonIgnore]
        public Item? Item { get; set; }

        // Added unit of measurement
        public decimal AmountUsed { get; set; }

        //Units of measurement
        public AmountUsedUnitOfMeasurement? AmountUsedMeasurement { get; set; } // How Much Of the item Was used to make the product
        public enum AmountUsedUnitOfMeasurement
        {
            Count,
            Liter,
            Millileter,
            Kilogram,
            Grams,
        }


    }


}
