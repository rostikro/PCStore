using Microsoft.AspNetCore.Identity;

namespace PCStore.Models;

public partial class User : IdentityUser
{
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}