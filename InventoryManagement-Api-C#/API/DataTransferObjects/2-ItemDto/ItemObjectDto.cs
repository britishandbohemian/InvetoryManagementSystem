
using System.ComponentModel.DataAnnotations;
using static InventoryAPI.Model.ItemsModel.Item;

namespace InventoryAPI.DataTransferObjects.ItemDto
{
    public class BaseItemDTO
    {
        [Required]
        public string ItemName { get; set; }
        public int ItemId { get; set; }


        [Required]
        public int CategoryId { get; set; }

        [Required]
        public ItemUnitsOfMeasurement UnitOfMeasurement { get; set; }

        //Not Know On creation
        public decimal? PricePerUnit { get; set; }


        //Not Know On creation
        public int? PiecesPerUnit { get; set; }

        //Not Know On Creation
        public DateTimeOffset? SellByDate { get; set; }

        [Required]
        public int MinimumThreshold { get; set; }

        [Required]
        public int MaximumThreshold { get; set; }

        //Not Know On Creation
        [Required]
        public int SupplierId { get; set; }



        public ItemStatus Status { get; set; } = ItemStatus.Active;
    }

    public class CreateItemDTO : BaseItemDTO
    {
        public int UnitsInInventory { get; set; } = 0;
    }

    public class UpdateItemDTO : BaseItemDTO
    {
        public int ItemId { get; set; }
        public int UnitsInInventory { get; set; }
    }

    public class ReadItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasurementName { get; set; }
        public DateTimeOffset? SellByDate { get; set; }
        public int UnitsInInventory { get; set; }
        public int PiecesPerUnit { get; set; }
        public decimal PricePerPiece { get; set; }
        public int MinimumThreshold { get; set; }
        public int MaximumThreshold { get; set; }
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public bool AvalibleOnExtra { get; set; }
        public ItemStatus Status { get; set; }
    }
}
