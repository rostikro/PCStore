using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class ShoppingCartProduct
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProdcutId { get; set; }

    public int Quantity { get; set; }

    public virtual ShoppingCart? Cart { get; set; }

    public virtual Product? Prodcut { get; set; }
}
