using System;
using System.Collections.Generic;

namespace ApplShopAPI.Model;

public partial class UserRole
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
