// Import necessary namespaces
using InventoryAPI.Data;
using InventoryAPI.DTO.User;
using InventoryAPI.Model.UserModels;
using Microsoft.AspNetCore.Mvc; // Importing ASP.NET Core MVC related classes
using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for database interaction
using System.Security.Cryptography;
using System.Text;

// Authenticate User
//This Is Complete user can Login And Register
namespace InventoryAPI.Controllers
{
    // The AuthController handles authentication-related API endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context; // Database context to interact with the database

        public AuthController(DatabaseContext context)
        {
            _context = context; // Constructor to inject the DatabaseContext using Dependency Injection
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<User>> RegisterUser(UserRegisterDto registerDto)
        {
            // Check if the username is already in use
            if (await _context.Users.AnyAsync(x => x.Username == registerDto.Username))
            {
                return BadRequest("Username already in use");
            }

            // Validate the role value
            if (registerDto.Role is < 0 or > (UserRole)3)
            {
                return BadRequest("Invalid role value");
            }

            // Map the integer role value to the UserRole enumeration
            UserRole userRole = (UserRole)registerDto.Role;

            // Create a new user from the registration DTO
            var user = new User
            {
                Username = registerDto.Username,
                UserPassword = HashPassword(registerDto.UserPassword), // Hashing the password
                UserEmail = registerDto.UserEmail,
                UserContact = registerDto.UserContact,
                Role = userRole // Assigning the mapped role
            };

            _context.Users.Add(user); // Add the new user to the database context
            await _context.SaveChangesAsync(); // Save changes to the database

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user); // Return the newly created user
        }

        //Hash the password
        private string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Combine the password and salt
            byte[] passwordAndSalt = Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));

            // Compute the hash
            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(passwordAndSalt);
            }

            // Combine the hash and salt
            byte[] hashAndSalt = new byte[hash.Length + salt.Length];
            Array.Copy(hash, 0, hashAndSalt, 0, hash.Length);
            Array.Copy(salt, 0, hashAndSalt, hash.Length, salt.Length);

            return Convert.ToBase64String(hashAndSalt);
        }

        // This endpoint allows retrieving a user by their ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id); // Find the user with the provided ID in the database

            if (user == null)
            {
                return NotFound(); // If user is not found, return 404 Not Found
            }

            return user; // Return the found user
        }

        //Log In User
        [HttpPost("LoginUser")]
        public async Task<ActionResult<User>> LoginUser(UserLoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return NotFound();
            }

            // Extract the hash and salt from the stored hash
            byte[] hashAndSalt = Convert.FromBase64String(user.UserPassword);
            byte[] storedHash = new byte[32];
            byte[] storedSalt = new byte[16];
            Array.Copy(hashAndSalt, 0, storedHash, 0, 32);
            Array.Copy(hashAndSalt, 32, storedSalt, 0, 16);

            // Combine the input password and stored salt
            byte[] passwordAndSalt = Encoding.UTF8.GetBytes(loginDto.UserPassword + Convert.ToBase64String(storedSalt));

            // Compute the hash of the input password
            byte[] inputHash;
            using (var sha256 = SHA256.Create())
            {
                inputHash = sha256.ComputeHash(passwordAndSalt);
            }

            // Compare the hashes
            if (!storedHash.SequenceEqual(inputHash))
            {
                return Unauthorized();
            }

            return user;
        }


    }
}
