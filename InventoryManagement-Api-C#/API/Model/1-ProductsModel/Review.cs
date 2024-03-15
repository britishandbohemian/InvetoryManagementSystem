using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.Model.ProductsModel
{
    public class Review
    {
        // Unique identifier for each review
        public int ReviewId { get; set; }

        // Reviewer's name or identifier
        public string ReviewerName { get; set; }

        // Rating provided in the review
        [Range(1, 5)] // Assuming a 1-5 rating scale
        public int Rating { get; set; }

        // The text content of the review
        public string Comment { get; set; }

        // The date when the review was submitted
        public DateTime DatePosted { get; set; }

        // Foreign key for the product that this review belongs to
        public int ProductId { get; set; }

        // Navigation property to the product
        public virtual Product Product { get; set; }

        // Additional properties (like Reviewer's Email, Verified Purchase, etc.) can be added here
    }
}
