
using InventoryAPI.DataTransferObjects.ItemDto;
using InventoryAPI.DTOs;
using InventoryAPI.Model.SupplierModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DataTransferObjects.SupplierDto
{
    // Base DTO for common properties
    public class BaseSupplierDto
    {
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

        [Range(1, 10)]
        public int Rating { get; set; }
    }

    // For creating a new supplier
    public class CreateSupplierDto : BaseSupplierDto { }

    // For updating an existing supplier
    public class UpdateSupplierDto : BaseSupplierDto
    {
        [Required]
        public int SupplierId { get; set; }
    }


    //instead of specifying the products in the dto i should be able to specify the orderproduct Id and create the supplier order
    public class CreateSupplierOrderDTO
    {
        [Required]
        public int LeadTimeFromSupplier { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public DateTime ExpectedDeliveryDate { get; set; }

        public int PlacedByUserId { get; set; }

        [Required]
        public List<int> OrderProductIds { get; set; }  // Using a list to accept multiple OrderedProduct IDs

        [Required]
        public List<int> OrderItemIds { get; set; }  // Using a list to accept multiple OrderedItem IDs
    }


    public class OrderProductDTO
    {
        [Required]
        public int ProductId { get; set; }

        public int? SupplierOrderId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Units ordered must be greater than 0.")]
        public int UnitsOrdered { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal TotalCostOfOrder { get; set; }

        public int PiecesPerUnit { get; set; }


        public DateTime OrderDate { get; set; }
    }


    public class OrderItemDTO
    {
        [Required]
        public int ItemId { get; set; }
        public int? SupplierOrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity ordered must be greater than 0.")]
        public int UnitsOrdered { get; set; }

        public decimal TotalCostOfOrder { get; set; }

        public int PiecesPerUnit { get; set; }

        public DateTime OrderDate { get; set; }
    }


    public class ConfirmOrderArrivalDTO
    {
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }
        public List<SimpleReceivedProductDTO> ProductsReceived { get; set; }
        public List<SimpleReceivedItemDTO> ItemsReceived { get; set; }
        public DateTime ReceivedDate { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int SupplierRating { get; set; }
    }

    public class SimpleReceivedProductDTO
    {
        public int ProductId { get; set; }
        public int QuantityReceived { get; set; }
        public DateTimeOffset? SellByDate { get; set; } // Using nullable in case it's not always provided
    }

    public class SimpleReceivedItemDTO
    {
        public int ItemId { get; set; }
        public int QuantityReceived { get; set; }
        public DateTimeOffset? SellByDate { get; set; } // Using nullable in case it's not always provided
    }


    public class ReceivedProductDTO
    {
        public int ProductId { get; set; }

        public ReadProductDTO Product { get; set; } // Reference to ReadProductDTO

        public int QuantityReceived { get; set; }

        public DateTimeOffset SellByDate { get; set; }
    }

    public class ReceivedItemDTO
    {
        public int ItemId { get; set; }

        public ReadItemDto Item { get; set; } // Reference to ReadItemDto

        public int QuantityReceived { get; set; }

        public DateTimeOffset SellByDate { get; set; }
    }

    public class OrderedProductDetailsDTO
    {
        public int OrderedProductId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitsOrdered { get; set; }
        public decimal TotalCostOfOrder { get; set; }
        public OrderProduct.OrderStatus? Status { get; set; }



    }

    public class OrderedItemDetailsDTO
    {
        public int OrderedItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int UnitsOrdered { get; set; }
        public decimal TotalCostOfOrder { get; set; }
        public OrderItem.OrderItemStatus? Status { get; set; }
    }


    // DTO to represent the detailed information for a supplier order
    // DTO to represent the detailed information for a supplier order
    public class SupplierOrderDetailsDTO
    {
        public int SupplierOrderId { get; set; }
        public string SupplierName { get; set; }
        public Supplier Supplier { get; set; }
        public decimal TotalCostOfOrder { get; set; }
        public int LeadTimeFromSupplier { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public int PlacedByUserId { get; set; }
        public int? CheckedByUserId { get; set; }
        public int Rating { get; set; }
        public SupplierOrder.StatusOfOrder Status { get; set; }  // Directly use the enum here
        public List<OrderedProductDetailsDTO> OrderProducts { get; set; }
        public List<OrderedItemDetailsDTO> OrderItems { get; set; }
    }

    public class AllOrderedDetailsDTO
    {
        public List<OrderedProductDetailsDTO> OrderedProducts { get; set; }
        public List<OrderedItemDetailsDTO> OrderedItems { get; set; }
    }


}
