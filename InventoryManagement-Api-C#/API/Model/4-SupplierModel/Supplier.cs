using InventoryAPI.Model.ItemsModel;
using InventoryAPI.Model.ProductsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryAPI.Model.SupplierModel
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        [StringLength(15)]  // Assuming a max length for phone numbers
        public string PhoneNumber { get; set; }

        [StringLength(30)]
        public string ContactPerson { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        public SupplierStatus Status { get; set; }
        public enum SupplierStatus
        {
            Active,
            Inactive
        }

        [JsonIgnore]
        public virtual ICollection<SupplierOrder> SupplierOrders { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int? Rating { get; set; }

        public DateTime CreatedAt { get; set; }  // Timestamp for creation
        public DateTime UpdatedAt { get; set; }  // Timestamp for last update
    }
}
