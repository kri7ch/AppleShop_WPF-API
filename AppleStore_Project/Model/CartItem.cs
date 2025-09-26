using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class CartItem
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    public uint ProductId { get; set; }

    public ushort Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
