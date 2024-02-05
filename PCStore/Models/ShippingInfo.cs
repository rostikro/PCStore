using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class ShippingInfo
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string Street { get; set; } = null!;

    public string Apartment { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
