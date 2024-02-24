using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCStore.Models;

public partial class Product
{
    public int Id { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Name")]
    public string Name { get; set; } = null!;

    [Display(Name = "Price")]
    public int Price { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Display(Name = "Category")]
    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();

    public virtual ICollection<SpecsOption> SpecsOptions { get; set; } = new List<SpecsOption>();
}
