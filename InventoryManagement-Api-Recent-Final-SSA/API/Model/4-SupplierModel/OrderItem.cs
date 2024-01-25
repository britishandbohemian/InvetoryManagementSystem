using InventoryAPI.Model.ItemsModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderedItemId { get; set; }



    [ForeignKey("SupplierOrderId")]
    public int? SupplierOrderId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    [JsonIgnore]
    public virtual Item Item { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity ordered must be greater than 0.")]
    [DefaultValue(0)]
    public int UnitsOrdered { get; set; }

    public decimal UnitPrice => TotalCostOfOrder / UnitsOrdered;

    //How Many Pieces Are In A Unit 
    public int PiecesPerUnit { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Total cost of order must be greater than 0.")]
    public decimal TotalCostOfOrder { get; set; }

    // Total number of individual pieces/items in the order
    public long TotalPiecesOrdered => UnitsOrdered * PiecesPerUnit; // Derived property

    public int? Active { get; set; }

    public DateTime OrderDate { get; set; }

    //How Much Does each piece cost you
    public decimal CostPricePerPiece => TotalPiecesOrdered != 0 ? TotalCostOfOrder / TotalPiecesOrdered : 0;  // Derived property

    public OrderItemStatus Status { get; set; }

    public OrderItem()
    {
        // Setting the default status to Pending
        Status = OrderItemStatus.Pending;
    }
    public enum OrderItemStatus
    {
        Pending,
        Ordered,  // New status
        Shipped,
        Received,
        Cancelled
    }





}
