using InventoryAPI.Data;
using InventoryAPI.DataTransferObjects.SupplierDto;
using InventoryAPI.Model.SupplierModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        //Data Base Context
        public SuppliersController(DatabaseContext context)
        {
            _context = context;
        }


        // Add Supplier
        [HttpPost]
        public async Task<IActionResult> CreateSupplier(CreateSupplierDto dto)
        {
            // Validate the input using model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a supplier with the same name already exists
            var existingSupplierWithName = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierName == dto.SupplierName);
            if (existingSupplierWithName != null)
            {
                return Conflict("A supplier with the same name already exists.");
            }

            // Check if a supplier with the same email already exists
            var existingSupplierWithEmail = await _context.Suppliers.FirstOrDefaultAsync(s => s.EmailAddress == dto.EmailAddress);
            if (existingSupplierWithEmail != null)
            {
                return Conflict("A supplier with the same email address already exists.");
            }

            // Create a new supplier object using the DTO
            var supplier = new Supplier
            {
                SupplierName = dto.SupplierName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                ContactPerson = dto.ContactPerson,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Rating = dto.Rating
            };

            // Add the supplier to the database context and save changes
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            // Return a success response along with a message
            return Ok(new { message = "Okay" });
        }

        // Get Supplier With Components and products
        [HttpGet]
        public IActionResult GetAllSuppliers()
        {
            // Query the suppliers along with their related components and products
            var suppliers = _context.Suppliers
                .Include(s => s.Items)
                .Include(s => s.Products)
                .ToList();

            // Return the suppliers
            return Ok(suppliers);
        }

        // Get By Id With Components and products
        [HttpGet("{id}")]
        public IActionResult GetSupplierById(int id)
        {
            // Find the supplier by ID, including related components and products
            var supplier = _context.Suppliers
                .Include(s => s.Items)
                .Include(s => s.Products)
                .FirstOrDefault(s => s.SupplierId == id);

            // If the supplier is not found, return a not found response
            if (supplier == null) return NotFound();

            // Return the supplier
            return Ok(supplier);
        }

        // Put Update the supplier info Only
        [HttpPut("{id}")]
        public IActionResult UpdateSupplier(int id, UpdateSupplierDto dto)
        {
            // Find the supplier by ID
            var supplier = _context.Suppliers.Find(id);

            // If the supplier is not found, return a not found response
            if (supplier == null) return NotFound();

            // Update the supplier's properties using the DTO
            supplier.SupplierName = dto.SupplierName;
            supplier.EmailAddress = dto.EmailAddress;
            supplier.PhoneNumber = dto.PhoneNumber;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Street = dto.Street;
            supplier.City = dto.City;
            supplier.State = dto.State;
            supplier.PostalCode = dto.PostalCode;
            supplier.Rating = dto.Rating;

            // Save the changes
            _context.SaveChanges();

            // Return a success response
            return Ok("Succes");
        }

        // Delete Supplier
        [HttpDelete("{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            // Find the supplier by ID
            var supplier = _context.Suppliers.Find(id);

            // If the supplier is not found, return a not found response
            if (supplier == null) return NotFound();

            // Remove the supplier from the database context and save changes
            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();

            // Return a success response
            return Ok();
        }
    }
}
