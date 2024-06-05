using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public int Total { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
