using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplShopAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ApplShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppleStoreContext _context;

        public UserController(AppleStoreContext context)
        {
            _context = context;
        }

        public class RegisterRequest
        {
            [Required][EmailAddress] public string Email { get; set; } = string.Empty;
            [Required][MinLength(6)] public string Password { get; set; } = string.Empty;
        }   

        public class LoginRequest
        {
            [Required] public string Email { get; set; } = string.Empty;
            [Required] public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return BadRequest("Пользователь с таким email уже существует");

                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    RoleId = 1,
                    RegistrationDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Регистрация успешна",
                    UserId = user.Id,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                    return Unauthorized("Неверный email или пароль");

                if (user.IsActive != true)
                    return BadRequest("Аккаунт деактивирован");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(uint id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return NotFound("Пользователь не найден");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
        
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(uint id, [FromBody] ProfileUpdateRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound("Пользователь не найден");

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
                { 
                    if (!new EmailAddressAttribute().IsValid(request.Email))
                        return BadRequest("Некорректный формат email");

                    var exists = await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != id);
                    if (exists)
                        return BadRequest("Email уже используется другим пользователем");

                    user.Email = request.Email!;
                }

                user.Phone = request.Phone;
                user.DeliveryAddress = request.DeliveryAddress;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Профиль успешно обновлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        public class ProfileUpdateRequest
        {
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? DeliveryAddress { get; set; }
        }

        public class UpdateIsActiveRequest
        {
            public bool IsActive { get; set; }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + "SALT_SECRET");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }

        [HttpGet("getid/{email}")]
        public async Task<ActionResult<int>> GetUserIdByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();

            return Ok((int)user.Id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] string? q)
        {
            var queryable = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var lower = q.ToLower();
                queryable = queryable.Where(u =>
                    u.Email.ToLower().Contains(lower) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(lower)) ||
                    (u.DeliveryAddress != null && u.DeliveryAddress.ToLower().Contains(lower))
                );
            }

            var users = await queryable
                .OrderBy(u => u.Id)
                .ToListAsync();

            return Ok(users);
        }
        
        [HttpPut("{id}/is-active")]
        public async Task<IActionResult> UpdateIsActive(uint id, [FromBody] UpdateIsActiveRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Пользователь не найден");

            user.IsActive = request.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Статус активности обновлён", IsActive = user.IsActive });
        }
    }
}