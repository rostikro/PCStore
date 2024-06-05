using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCStore.Models;

public partial class Address
{
    public int Id { get; set; }
    
    public string FullAddress
    {
        get { return Street + " " + Apartment + ", " + City; }
    }

    public string UserId { get; set; } = null!;

    [Display(Name="Вулиця")]
    public string Street { get; set; } = null!;

    [Display(Name="Адрес 2")]
    public string Apartment { get; set; } = null!;

    [Display(Name="Місто")]
    public string City { get; set; } = null!;

    [Display(Name="Область")]
    public string Province { get; set; } = null!;

    [Display(Name="Zip")]
    public string PostCode { get; set; } = null!;

    [Display(Name="Країна")]
    public string Country { get; set; } = null!;

    public sbyte IsDefault { get; set; }

    public virtual User User { get; set; } = new User();
}
