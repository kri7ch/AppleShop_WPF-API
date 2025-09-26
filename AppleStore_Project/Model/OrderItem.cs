using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class OrderItem
{
    public uint Id { get; set; }

    public uint OrderId { get; set; }

    public uint ProductId { get; set; }

    public ushort Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
