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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Name is required");

            var product = new Product
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                ImageCode = request.ImageCode,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                StatusId = request.StatusId == 0 ? 1 : request.StatusId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(uint id, [FromBody] Product request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.ImageCode = request.ImageCode;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            if (request.StatusId != 0)
                product.StatusId = request.StatusId;

            await _context.SaveChangesAsync();
            return Ok(product);
        }
    }
}