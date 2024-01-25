using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class OrderProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderedProductId { get; set; }

    [ForeignKey("SupplierOrderId")]
    public int? SupplierOrderId { get; set; }

    [ForeignKey("ProductId")]
    public int ProductId { get; set; }

    public decimal CostPricePerPiece { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; }

    private int _unitsOrdered;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Units ordered must be greater than 0.")]
    public int UnitsOrdered
    {
        get => _unitsOrdered;
        set
        {
            _unitsOrdered = value;
            CalculateTotalPiecesOrdered();
        }
    }

    private int _piecesPerUnit;

    public int PiecesPerUnit
    {
        get => _piecesPerUnit;
        set
        {
            _piecesPerUnit = value;
            CalculateTotalPiecesOrdered();
        }
    }

    public long TotalPiecesOrdered { get; private set; }

    public DateTime OrderDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total cost of order must be greater than 0.")]
    public decimal TotalCostOfOrder { get; set; }

    public decimal UnitPrice => TotalCostOfOrder / UnitsOrdered;

    public int? Active { get; set; }

    public OrderStatus Status { get; set; }

    public OrderProduct()
    {
        // Setting the default status to Pending
        Status = OrderStatus.Pending;
    }

    public enum OrderStatus
    {
        Pending,
        Ordered,  // New status
        Shipped,
        Received,
        Cancelled
    }

    private void CalculateTotalPiecesOrdered()
    {
        TotalPiecesOrdered = _unitsOrdered * _piecesPerUnit;
    }
}
