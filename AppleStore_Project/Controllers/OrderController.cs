using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;
using System.ComponentModel.DataAnnotations;

namespace ApplShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppleStoreContext _context;
        private readonly ApplShopAPI.Services.ReceiptService _receiptService;
        private readonly ApplShopAPI.Services.EmailService _emailService;

        public OrderController(AppleStoreContext context, ApplShopAPI.Services.ReceiptService receiptService, ApplShopAPI.Services.EmailService emailService)
        {
            _context = context;
            _receiptService = receiptService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders.Include(o => o.User).Include(o => o.Status).Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(uint userId)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!exists) return NotFound("Пользователь не найден");

            var orders = await _context.Orders.Where(o => o.UserId == userId).Include(o => o.Status).Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();

            return Ok(orders);
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) return NotFound("Пользователь не найден");

            if (request.OrderItems == null || request.OrderItems.Count == 0)
                return BadRequest("Необходимо указать товары");

            var payment = (request.PaymentMethod ?? "").Trim();
            bool isCard = string.Equals(payment, "Card", StringComparison.OrdinalIgnoreCase);
            bool isCash = string.Equals(payment, "Cash", StringComparison.OrdinalIgnoreCase);
            if (!isCard && !isCash)
                return BadRequest("Некорректный способ оплаты");

            var productIds = request.OrderItems.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            if (products.Count != productIds.Count)
                return BadRequest("Один или несколько товаров не найдены");

            foreach (var item in request.OrderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (item.Quantity == 0)
                    return BadRequest("Количество должно быть больше нуля");
                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Недостаточно товара на складе: {product.Name}");
            }

            var totalAmount = request.OrderItems.Sum(i => i.Price * i.Quantity);

            uint statusId = isCard ? 1u : 2u;

            var order = new Order
            {
                UserId = request.UserId,
                DeliveryAddress = request.DeliveryAddress,
                TotalAmount = totalAmount,
                OrderDate = DateTime.UtcNow,
                StatusId = statusId
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in request.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderItems.Add(orderItem);

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity = product.StockQuantity > item.Quantity ? product.StockQuantity - item.Quantity : 0;

                    if (product.StockQuantity == 0)
                    {
                        var outOfStockItems = await _context.CartItems.Where(ci => ci.ProductId == product.Id).ToListAsync();
                        if (outOfStockItems.Count > 0)
                        {
                            _context.CartItems.RemoveRange(outOfStockItems);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            var cartItems = await _context.CartItems.Where(ci => ci.UserId == request.UserId).ToListAsync();
            if (cartItems.Count > 0)
            {
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            var created = await _context.Orders.Include(o => o.User).Include(o => o.Status).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstAsync(o => o.Id == order.Id);

            created.PaymentMethod = payment;

            try
            {
                var pdf = _receiptService.Generate(created);
                var recipient = created.User?.Email ?? user.Email;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendReceiptAsync(recipient!, pdf, created.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка отправки письма: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки письма: {ex.Message}");
            }

            return Ok(created);
        }
    }
}