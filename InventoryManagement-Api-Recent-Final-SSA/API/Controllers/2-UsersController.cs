using InventoryAPI.Data;
using InventoryAPI.Model.UserModels;
using Microsoft.AspNetCore.Mvc; // Importing ASP.NET Core MVC related classes

// Namespace for the Inventory API's Controllers
namespace InventoryAPI.Controllers
{
    // The UsersController handles CRUD operations for the User entity
    //Tested And Approved Working Not To be changed
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context; // Database context to interact with the database

        public UsersController(DatabaseContext context)
        {
            _context = context; // Constructor to inject the DatabaseContext using Dependency Injection
        }

        // Create a user
        // This endpoint allows creating a new user by passing the user details in the request body
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            try
            {
                _context.Users.Add(user); // Add the new user to the database context
                _context.SaveChanges(); // Save changes to the database

                return Ok("User created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // If an error occurs, return 500 Internal Server Error
            }
        }

        // Get all users
        // This endpoint allows retrieving a list of all users in the database
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users.ToList(); // Retrieve all users from the database
                return Ok(users); // Return the list of users
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // If an error occurs, return 500 Internal Server Error
            }
        }

        // Get a specific user by ID
        // This endpoint allows retrieving a user by providing their ID in the URL
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id); // Find the user with the provided ID in the database

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(user); // Return the found user
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // If an error occurs, return 500 Internal Server Error
            }
        }

        // Update a user
        // This endpoint allows updating an existing user by providing their ID in the URL and the updated user details in the request body
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            try
            {
                var existingUser = _context.Users.Find(id); // Find the existing user with the provided ID in the database

                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                // Update the user's properties.
                existingUser.Username = user.Username;
                existingUser.UserPassword = user.UserPassword;
                existingUser.Role = user.Role;

                _context.SaveChanges(); // Save changes to the database
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // If an error occurs, return 500 Internal Server Error
            }
        }

        // Delete a user
        // This endpoint allows deleting an existing user by providing their ID in the URL
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.Find(id); // Find the user with the provided ID in the database

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _context.Users.Remove(user); // Remove the user from the database context
                _context.SaveChanges(); // Save changes to the database

                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // If an error occurs, return 500 Internal Server Error
            }
        }
        [HttpGet("employee-sales")]
        public IActionResult GetAllEmployeesOrderedBySales()
        {
            try
            {
                var employees = _context.Users
                    .Where(u => u.Role == UserRole.Employee || u.Role == UserRole.Manager) // Filter by role
                    .Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        TotalSalesAmount = _context.Sales
                            .Where(s => s.UserId == u.UserId)
                            .Sum(s => s.Total),
                        NumberOfSales = _context.Sales
                            .Count(s => s.UserId == u.UserId)
                    })
                    .OrderByDescending(e => e.TotalSalesAmount)
                    .ThenByDescending(e => e.NumberOfSales)
                    .ToList();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
