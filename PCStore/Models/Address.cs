using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class Address
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Street { get; set; } = null!;

    public string Apartment { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string Country { get; set; } = null!;

    public sbyte IsDefault { get; set; }

    public virtual User User { get; set; } = null!;
}
