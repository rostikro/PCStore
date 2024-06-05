using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public string? Description { get; set; }

    public int Stock { get; set; }

    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();

    public virtual ICollection<SpecsOption> SpecsOptions { get; set; } = new List<SpecsOption>();
}
