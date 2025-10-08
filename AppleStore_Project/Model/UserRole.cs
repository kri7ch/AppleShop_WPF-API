using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApplShopAPI.Model;

public partial class UserRole
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
