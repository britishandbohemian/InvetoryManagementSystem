using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductExpirationEvent
{
    //Keeps Track Of Expiring Products
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExpirationEventId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }

    public DateTimeOffset ExpirationDate { get; set; }
    public int UnitsInInventoryAtExpiration { get; set; }
}
