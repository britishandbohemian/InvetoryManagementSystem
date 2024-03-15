using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.SupplierModel
{
    public class SupplierOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierOrderId { get; set; }



        [Required]
        public decimal TotalCostOfOrder { get; set; }

        [Required]
        public int LeadTimeFromSupplier { get; set; }

        [ForeignKey("UserId")]
        public int? PlacedByUserId { get; set; }  // User who placed the order

        //Who Checked The Order
        public int? CheckedByUserId { get; set; }  // User who checked the order upon arrival

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }  // Navigation property for Supplier

        public StatusOfOrder Status { get; set; } = StatusOfOrder.WaitingForSupplierToConfirm;

        public enum StatusOfOrder
        {
            Arrived,
            WaitingForSupplierToConfirm,
            Cancelled,
            EnRoute,
            Confirmed,
            Rejected
        }


        public DateTime OrderDate { get; set; }

        //Calculated using Lead time of supplier
        [Required]
        public DateTime ExpectedDeliveryDate { get; set; }

        public DateTime? ActualDeliveryDate { get; set; }

        public string? TrackingNumber { get; set; }

        public int Rating { get; set; }

        //Given the orderItem Id And the Order Product ids Place those orders inside these two variables
        //Then Set The Supplier OrderId's in those tables
        [JsonIgnore]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
