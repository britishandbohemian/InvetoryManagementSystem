using InventoryAPI.DataTransferObjects.ItemDto;
using InventoryAPI.Model.LinkModels;
using InventoryAPI.Model.SupplierModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.ItemsModel
{
    // Defines an Item in the inventory
    public class Item
    {
        // Unique identifier for the Item
        public int ItemId { get; set; }

        // Name of the Item (e.g., "Box of Pens"). Must be provided and have a maximum length of 50 characters.
        [Required]
        [StringLength(50)]
        public string ItemName { get; set; }



        // The number of boxes or packets of this item currently in the inventory. Defaults to 0. (e.g., 5 boxes of pens)
        [DefaultValue(0)]
        public int UnitsInInventory { get; set; } = 0;

        // Price for each box or packet of the item (e.g., $10 per box of pens). 
        public decimal? PricePerUnit { get; set; } = 0;

        // Price for each individual piece within the box or packet. (e.g., if item represents a box of pens, this could be the price for one pen)
        public decimal? PricePerPiece { get; set; } = 0;



        // Number of individual pieces within a box or packet. (e.g., if item represents a box of pens, this could be the number of pens in one box)
        public int? PiecesPerUnit { get; set; } = 0;

        // Minimum number of boxes or packets to have in inventory. (e.g., Always maintain at least 5 boxes of pens in stock)
        [Required]
        [DefaultValue(0)]
        public int MinimumThreshold { get; set; }

        // Maximum number of boxes or packets to have in inventory. (e.g., Do not have more than 100 boxes of pens in stock)
        [Required]
        [DefaultValue(0)]
        public int MaximumThreshold { get; set; }

        // Current status of the item (e.g., Active, Sold Out, etc.)
        [DefaultValue(ItemStatus.Active)]
        public ItemStatus Status { get; set; }

        // Enum defining possible item statuses
        public enum ItemStatus
        {
            Active,
            Inactive,
            SoldOut,
            Expired,
            Discontinued
        }

        // Unit of measurement for the items within the box or packet (e.g., Liter for liquid items, Grams for grain items)
        public ItemUnitsOfMeasurement UnitOfMeasurement { get; set; }

        // Enum defining possible units of measurement
        public enum ItemUnitsOfMeasurement
        {
            Liter,
            Millileter,
            Kilogram,
            Grams,
        }

        // Date when the items within the box or packet expire (e.g., expiry date for a medicine packet)
        public DateTimeOffset? SellByDate { get; set; }

        // Identifier for the supplier of this item
        [ForeignKey("SupplierId")]
        public int? SupplierId { get; set; }
        // Navigation property for Supplier. (This allows access to Supplier details for this item)
        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }

        // Date when the price of the item was last changed
        public DateTime? LastPriceChangeDate { get; set; }
        // The price of the box or packet before the most recent price change (e.g., Price was $10 before it changed to $12 for a box)
        public decimal? PreviousPricePerPacket { get; set; } = 0;

        // Navigation property for a collection of Orders that include this item
        public virtual ICollection<OrderItem> OrderedItems { get; set; }
        // Navigation property for a collection of Products linked to this item
        public ICollection<ProductToItem> LinkedProducts { get; set; }

        public static explicit operator Item(Task<ActionResult<ReadItemDto>> v)
        {
            throw new NotImplementedException();
        }

        public bool? AvalibleOnExtra { get; set; }   
    }

}
