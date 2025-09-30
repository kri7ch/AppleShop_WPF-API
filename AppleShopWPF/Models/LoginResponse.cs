namespace AppleShopWPF.Models
{
    public class LoginResponse
    {
        public UserData User { get; set; } = new UserData();
    }

    public class UserData
    {
        public uint Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}