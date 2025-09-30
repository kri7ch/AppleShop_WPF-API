using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppleStoreContext _context;

        public ProductController(AppleStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageCode = p.ImageCode,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync();

            return Ok(products);
        }
    }

    public class ProductDto
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageCode { get; set; }
        public decimal Price { get; set; }
        public uint StockQuantity { get; set; }
    }
}