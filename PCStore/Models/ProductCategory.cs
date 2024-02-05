﻿using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class ProductCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Spec> Specs { get; set; } = new List<Spec>();
}
