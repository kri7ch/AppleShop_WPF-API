using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ApplShopAPI.Model;


namespace AppleShopWPF.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5279/api/User";
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var registerData = new { Email = email, Password = password };
                var json = JsonSerializer.Serialize(registerData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/register", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Успешная регистрация!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка регистрации: {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { Email = email, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<User>(payload, _jsonOptions);

                    if (user != null)
                    {
                        AppState.CurrentUserId = (int)user.Id;
                        AppState.CurrentUserRoleId = (int)user.RoleId;
                    }

                    MessageBox.Show("Успешная авторизация!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка авторизации: {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5279/api/product");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<Product>>(content, _jsonOptions);
                    return products ?? new List<Product>();
                }
                return new List<Product>();
            }
            catch
            {
                return new List<Product>();
            }
        }

        public async Task<Product?> CreateProductAsync(uint categoryId, string name, decimal price, uint stockQuantity, string? imageCode)
        {
            try
            {
                var payload = new { CategoryId = categoryId, Name = name, Price = price, StockQuantity = stockQuantity, ImageCode = imageCode };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5279/api/product", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var created = JsonSerializer.Deserialize<Product>(body, _jsonOptions);
                    return created;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка создания продукта: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<Product?> UpdateProductAsync(uint id, uint categoryId, string name, decimal price, uint stockQuantity, string? imageCode)
        {
            try
            {
                var payload = new { CategoryId = categoryId, Name = name, Price = price, StockQuantity = stockQuantity, ImageCode = imageCode };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"http://localhost:5279/api/product/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var updated = JsonSerializer.Deserialize<Product>(body, _jsonOptions);
                    return updated;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка обновления продукта: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5279/api/user");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<List<User>>(content, _jsonOptions);
                    return users ?? new List<User>();
                }
                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/getid/{email}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(content, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch
            {
                //добавить позже
            }

            return 0;
        }

        public async Task<User?> GetProfileAsync(uint userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/profile/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<User>(content, _jsonOptions);
                    return profile;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка загрузки профиля: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<User?> UpdateProfileAsync(uint userId, string? email, string? phone, string? deliveryAddress)
        {
            try
            {
                var payload = new { Email = email, Phone = phone, DeliveryAddress = deliveryAddress };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{BaseUrl}/profile/{userId}", content);
                if (response.IsSuccessStatusCode)
                {
                    var refreshed = await GetProfileAsync(userId);
                    MessageBox.Show("Профиль обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return refreshed;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка обновления профиля: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<List<User>> GetUsersAsync(string? query = null)
        {
            var url = "http://localhost:5279/api/user";
            if (!string.IsNullOrWhiteSpace(query))
                url += $"?q={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<User>>(json, _jsonOptions)
                        ?? new List<User>();

            return users;
        }

        public async Task<bool> UpdateUserIsActiveAsync(uint userId, bool isActive)
        {
            var payload = new { IsActive = isActive };
            var content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5279/api/user/{userId}/is-active", content);
            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5279/api/order");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var orders = JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions);
                    return orders ?? new List<Order>();
                }
                return new List<Order>();
            }
            catch
            {
                return new List<Order>();
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(uint userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5279/api/order/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var orders = JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions);
                    return orders ?? new List<Order>();
                }
                return new List<Order>();
            }
            catch
            {
                return new List<Order>();
            }
        }

        public async Task<Order?> CreateOrderAsync(uint userId, string deliveryAddress, List<OrderItem> items, string paymentMethod)
        {
            try
            {
                var payload = new Order
                {
                    UserId = userId,
                    DeliveryAddress = deliveryAddress,
                    OrderItems = items,
                    PaymentMethod = paymentMethod
                };
                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5279/api/order", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var created = JsonSerializer.Deserialize<Order>(body, _jsonOptions);
                    return created;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка оформления заказа: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5279/api/category");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<Category>>(content, _jsonOptions);
                    return categories ?? new List<Category>();
                }
                return new List<Category>();
            }
            catch
            {
                return new List<Category>();
            }
        }

        public async Task<Category?> CreateCategoryAsync(string name, string? description)
        {
            try
            {
                var payload = new { Name = name, Description = description };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5279/api/category", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var created = JsonSerializer.Deserialize<Category>(body, _jsonOptions);
                    return created;
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        MessageBox.Show("Категория с таким названием уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка создания категории: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public async Task<Category?> UpdateCategoryAsync(uint id, string name, string? description)
        {
            try
            {
                var payload = new { Name = name, Description = description };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"http://localhost:5279/api/category/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var updated = JsonSerializer.Deserialize<Category>(body, _jsonOptions);
                    return updated;
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        MessageBox.Show("Категория с таким названием уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка обновления категории: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        // CART METHODS
        public async Task<List<CartItem>> GetCartAsync(uint userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5279/api/cart/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonSerializer.Deserialize<List<CartItem>>(content, _jsonOptions);
                    return items ?? new List<CartItem>();
                }
                return new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        public async Task<CartItem?> AddToCartAsync(uint userId, uint productId, ushort quantity = 1)
        {
            try
            {
                var payload = new { ProductId = productId, Quantity = quantity };
                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"http://localhost:5279/api/cart/{userId}/items", content);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var created = JsonSerializer.Deserialize<CartItem>(body, _jsonOptions);
                    return created;
                }
            }
            catch { }
            return null;
        }

        public async Task<bool> UpdateCartItemAsync(uint userId, uint productId, ushort quantity)
        {
            try
            {
                var payload = new { Quantity = quantity };
                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"http://localhost:5279/api/cart/{userId}/items/{productId}", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> RemoveCartItemAsync(uint userId, uint productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5279/api/cart/{userId}/items/{productId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> ClearCartAsync(uint userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5279/api/cart/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
