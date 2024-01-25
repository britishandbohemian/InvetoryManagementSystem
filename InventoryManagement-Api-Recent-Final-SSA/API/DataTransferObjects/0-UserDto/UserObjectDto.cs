using InventoryAPI.Model.UserModels;

using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTO.User
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(50)] // Limiting the username length
        public string Username { get; set; }

        // The user's password - 1234
        [Required]
        [DataType(DataType.Password)] // Specifying that this is a password field
        [StringLength(100, MinimumLength = 8)] // Ensuring a minimum and maximum password length
        public string UserPassword { get; set; }
    }

    public class UserRegisterDto
    {


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
        public UserRole Role { get; set; }
    }
}
