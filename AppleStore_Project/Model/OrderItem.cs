using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApplShopAPI.Model;

public partial class OrderItem
{
    public uint Id { get; set; }

    public uint OrderId { get; set; }

    public uint ProductId { get; set; }

    public ushort Quantity { get; set; }

    public decimal Price { get; set; }

    [JsonIgnore]
    public virtual Order? Order { get; set; }

    [JsonIgnore]
    public virtual Product? Product { get; set; }
}
