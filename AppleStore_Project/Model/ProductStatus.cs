﻿using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class ProductStatus
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
