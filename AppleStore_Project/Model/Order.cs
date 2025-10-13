using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApplShopAPI.Model;

public partial class Order
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    public uint StatusId { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    [NotMapped]
    public string? PaymentMethod { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [JsonIgnore]
    public virtual OrderStatus? Status { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
