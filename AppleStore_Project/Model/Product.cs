using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class Product
{
    public uint Id { get; set; }

    public uint CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageCode { get; set; }

    public decimal Price { get; set; }

    public uint StockQuantity { get; set; }

    public uint StatusId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ProductStatus Status { get; set; } = null!;
}
