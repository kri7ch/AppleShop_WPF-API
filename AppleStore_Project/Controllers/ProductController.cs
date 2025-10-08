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
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .ToListAsync();

            return Ok(products);
        }
    }
}