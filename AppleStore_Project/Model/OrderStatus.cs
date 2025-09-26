using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class OrderStatus
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
