using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;

namespace ApplShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppleStoreContext _context;

        public CartController(AppleStoreContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCart(uint userId)
        {
            var items = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
            return Ok(items);
        }

        public class AddCartItemRequest
        {
            public uint ProductId { get; set; }
            public ushort Quantity { get; set; } = 1;
        }

        [HttpPost("{userId}/items")]
        public async Task<ActionResult<CartItem>> AddItem(uint userId, [FromBody] AddCartItemRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) return NotFound("Product not found");

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

            if (existing != null)
            {
                existing.Quantity = (ushort)(existing.Quantity + (request.Quantity == 0 ? 1 : request.Quantity));
                await _context.SaveChangesAsync();
                return Ok(existing);
            }

            var item = new CartItem
            {
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity == 0 ? (ushort)1 : request.Quantity
            };
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        public class UpdateCartItemRequest
        {
            public ushort Quantity { get; set; }
        }

        [HttpPut("{userId}/items/{productId}")]
        public async Task<ActionResult<CartItem>> UpdateItem(uint userId, uint productId, [FromBody] UpdateCartItemRequest request)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
            if (item == null) return NotFound();

            if (request.Quantity == 0)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            item.Quantity = request.Quantity;
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveItem(uint userId, uint productId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
            if (item == null) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}