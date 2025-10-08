using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class Category
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
