using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class User
{
    public uint Id { get; set; }

    public uint RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual UserRole Role { get; set; } = null!;
}
