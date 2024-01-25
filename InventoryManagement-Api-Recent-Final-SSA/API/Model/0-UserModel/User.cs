

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAPI.Model.UserModels
{

    public class User
    {
        // Confirmed to be working Does Not Need to be changed // Tested Approved

        // The user ID for the database eg 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        // The user's name eg Kamogelo
        [Required]
        [StringLength(50)] // Limiting the username length
        public string Username { get; set; }

        // The user's password - 1234
        [Required]
        [DataType(DataType.Password)] // Specifying that this is a password field
        [StringLength(100, MinimumLength = 8)] // Ensuring a minimum and maximum password length
        public string UserPassword { get; set; }

        // The user's email
        [EmailAddress] // Validating the email format
        public string UserEmail { get; set; }

        // The user's phone number
        [Phone] // Validating the phone number format
        public string UserContact { get; set; }

        // The role of the user
        [Required]
        public UserRole Role { get; set; } // Changed to UserRole to define the enum outside the class
    }

    // Defining user roles as an enumeration
    public enum UserRole
    {
        Employee,      // Regular employee with limited access
        Manager,       // Manager with additional permissions
        MasterAdmin,   // Administrator with full control
        Supplier       // Supplier with access to specific supplier-related features
    }

}
