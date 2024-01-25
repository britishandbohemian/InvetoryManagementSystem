using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.CategoriesModel
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        // Derived properties for stats
        public decimal CategoryWorth => ProductsWorth + ItemsWorth;
        public decimal ProductsWorth => Products?.Sum(p => p.ProductSellingPrice) ?? 0;  // Assuming ProductSellingPrice is the worth of a product
        public decimal ItemsWorth => Items?.Sum(i => i.PricePerUnit * i.UnitsInInventory) ?? 0;  // Assuming PricePerPacket * PacketsInInventory is the worth of an item

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
        [JsonIgnore]
        public virtual ICollection<Item> Items { get; set; }


    }


}
