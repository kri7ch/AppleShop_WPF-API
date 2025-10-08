using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class Order
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    public uint StatusId { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
