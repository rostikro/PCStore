using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class ShoppingCart
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();

    public virtual User User { get; set; } = null!;
}
