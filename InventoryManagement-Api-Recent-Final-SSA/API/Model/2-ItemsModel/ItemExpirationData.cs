using InventoryAPI.Model.ItemsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ItemExpirationData
{
    [Key]
    [ForeignKey("Item")]
    public int ItemId { get; set; }

    public int TimesExpired { get; set; }

    public DateTime? LastExpirationDate { get; set; }

    public virtual Item Item { get; set; }
}
