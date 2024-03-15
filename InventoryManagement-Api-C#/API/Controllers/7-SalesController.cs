using InventoryAPI.Data;
using InventoryAPI.DTO.InventoryDto;
using InventoryAPI.Model.SalesModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventoryAPI.Model.SalesModel.Sale;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SalesController(DatabaseContext context)
        {
            _context = context;
        }

        //Ensure that when a sale is created the sub total is caluted then the total is caluated using the the tax percentage
        // POST: api/Sales/CreateSale
        [HttpPost("CreateSale")]
        public async Task<IActionResult> CreateSale(SaleDTO saleDTO)
        {
            if (saleDTO == null)
            {
                return BadRequest("Sale object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                UserId = saleDTO.UserId,
                AdditionalInfo = saleDTO.AdditionalInfo,
                PaymentType = (PaymentMethod)saleDTO.PaymentType,
            };

            sale.ProductsInSale = saleDTO.ProductsForSale.Select(p => new ProductInSale
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                //Get The price Of the Product from Data
                ProductSellingPrice = p.ProductSellingPrice
            }).ToList();

            // Calculate the Subtotal, Tax, and Total
            sale.Subtotal = sale.ProductsInSale.Sum(p => p.ProductSellingPrice * p.Quantity);
            sale.Tax = 0.1m * sale.Subtotal; // Assuming 10% tax, modify as per your requirement
            sale.Total = sale.Subtotal + sale.Tax;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Adjusting product levels after a sale
            foreach (var productInSale in sale.ProductsInSale)
            {
                // Adjust product inventory
                var product = await _context.Products.FindAsync(productInSale.ProductId);
                if (product != null)
                {
                    product.UnitsInInventory -= productInSale.Quantity;
                    product.LastSoldDate = DateTime.Now;
                }

                // Adjust component item inventory
                var productItems = await _context.ProductItems
                    .Where(pi => pi.ProductId == productInSale.ProductId)
                    .ToListAsync();

                // If the product has no components, continue to the next product
                if (!productItems.Any()) continue;

                foreach (var productItem in productItems)
                {
                    var item = await _context.Items.FindAsync(productItem.ItemId);
                    if (item != null)
                    {
                        decimal totalItemAmountUsed = productInSale.Quantity * productItem.AmountUsed;
                        item.UnitsInInventory -= (int)totalItemAmountUsed;

                        var componentInSale = new ComponentInSale
                        {
                            ProductInSaleId = productInSale.ProductInSaleId, // Now this ID is available
                            ItemId = productItem.ItemId,
                            AmountUsed = totalItemAmountUsed
                        };
                        _context.ComponentsInSale.Add(componentInSale);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sale Made and Product Levels Adjusted" });
        }

        [HttpGet("GetAllSales")]
        public async Task<IActionResult> GetAllSales()
        {
            List<Sale> sales = await _context.Sales
                .Include(s => s.ProductsInSale)
                    .ThenInclude(p => p.Product)
                .Include(s => s.ProductsInSale)
                    .ThenInclude(p => p.ComponentsInSale)
                        .ThenInclude(c => c.Item)
                .Include(s => s.User)  // Include the User object
                .ToListAsync();

            var saleDTOs = sales.Select(s => new SaleDTO
            {
                SaleId = s.SaleId,
                Total = s.Total,
                Subtotal = s.Subtotal,
                Tax = s.Tax,
                SaleDate = s.SaleDate,
                UserId = s.UserId,
                AdditionalInfo = s.AdditionalInfo,
                PaymentType = (SaleDTO.SaleDTOPaymentMethod)s.PaymentType,
                ProductsForSale = s.ProductsInSale.Select(p =>
                {
                    var productForSaleDTO = new SaleDTO.ProductForSaleDTO
                    {
                        ProductId = p.ProductId,
                        Quantity = p.Quantity,
                        ProductSellingPrice = p.ProductSellingPrice,
                        ComponentsInSale = p.ComponentsInSale.Select(c => new SaleDTO.ComponentForSaleDTO
                        {
                            ItemId = c.ItemId,
                            AmountUsed = c.AmountUsed
                        }).ToList()
                    };

                    return productForSaleDTO;
                }).ToList()
            }).ToList();

            return Ok(saleDTOs);
        }


    }
}
