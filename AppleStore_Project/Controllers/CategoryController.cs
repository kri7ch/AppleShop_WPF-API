using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;

namespace ApplShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppleStoreContext _context;

        public CategoryController(AppleStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category request)
        {
            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                return BadRequest("Требуется указать название");

            var exists = await _context.Categories.AnyAsync(c => c.Name == normalizedName);
            if (exists)
                return Conflict("Категория с таким названием уже существует");

            var category = new Category
            {
                Name = normalizedName!,
                Description = request.Description
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(uint id, [FromBody] Category request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var normalizedName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
                return BadRequest("Требуется указать название");

            var duplicate = await _context.Categories.AnyAsync(c => c.Id != id && c.Name == normalizedName);
            if (duplicate)
                return Conflict("Категория с таким названием уже существует");

            category.Name = normalizedName!;
            category.Description = request.Description;
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(uint id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound("Категория не найдена");

            var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();
            if (products.Count > 0)
            {
                var productIds = products.Select(p => p.Id).ToList();
                var hasOrderItems = await _context.OrderItems.AnyAsync(oi => productIds.Contains(oi.ProductId));
                if (hasOrderItems)
                    return Conflict("Невозможно удалить категорию, её товары присутствуют в заказах");

                var cartItems = await _context.CartItems.Where(ci => productIds.Contains(ci.ProductId)).ToListAsync();
                if (cartItems.Count > 0)
                    _context.CartItems.RemoveRange(cartItems);

                _context.Products.RemoveRange(products);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}