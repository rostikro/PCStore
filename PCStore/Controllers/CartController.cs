using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Controllers;

public class OrderViewModel
{
    public ShoppingCart ShoppingCart { get; set; }
    public int AddressId { get; set; }
}

public class CartController : Controller
{
    private readonly PCStoreDBContext _context;
    private readonly UserManager<User> _userManager;
    
    public CartController(PCStoreDBContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var cart = await _context.ShoppingCarts
            .Include(cart => cart.ShoppingCartProducts)
            .ThenInclude(c => c.Prodcut)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (cart == null)
        {
            await CreateCartAsync(user);
        }

        var addresses =
            ViewData["Addresses"] = new SelectList(_context.Addresses.Where(address => address.UserId == user.Id),
                "Id", "FullAddress");

        int totalPrice = 0;
        foreach (var cartProduct in cart.ShoppingCartProducts)
        {
            totalPrice += cartProduct.Quantity * cartProduct.Prodcut.Price;
        }
        ViewData["TotalPrice"] = totalPrice;
        ViewData["OrderViewModel"] = new OrderViewModel();
        
        return View(new OrderViewModel
        {
            ShoppingCart = cart
        });
    }

    public async Task<IActionResult> AddProduct(int? id)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(p => p.UserId == user.Id);
        
        if (cart == null)
        {
            await CreateCartAsync(user);
        }
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var cartProduct = new ShoppingCartProduct()
        {
            Cart = cart,
            Prodcut = product,
            Quantity = 1
        };

        var productInCart =
            await _context.ShoppingCartProducts.FirstOrDefaultAsync(p =>
                p.CartId == cart.Id && p.ProdcutId == product.Id);
        if (productInCart != null)
        {
            productInCart.Quantity++;
            _context.Update(productInCart);
        }
        else
        {
            _context.Add(cartProduct);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Product", new { id });
    }

    public async Task<IActionResult> RemoveProduct(int? id)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(p => p.UserId == user.Id);
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var productInCart =
            await _context.ShoppingCartProducts.FirstOrDefaultAsync(p =>
                p.CartId == cart.Id && p.ProdcutId == product.Id);
        
        if (productInCart == null)
        {
            return NotFound();
        }
        
        _context.Remove(productInCart);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangeProductQuantity(int? id, int quantity)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(p => p.UserId == user.Id);
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var productInCart =
            await _context.ShoppingCartProducts.FirstOrDefaultAsync(p =>
                p.CartId == cart.Id && p.ProdcutId == product.Id);

        if (productInCart == null)
        {
            return NotFound();
        }
        
        productInCart.Quantity+=quantity;
        if (productInCart.Quantity == 0)
        {
            _context.Remove(productInCart);
        }
        else
        {
            _context.Update(productInCart);
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> MakeOrder(OrderViewModel viewModel)
    {
        var addressId = viewModel.AddressId;
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        var cart = await _context.ShoppingCarts.Include(p => p.ShoppingCartProducts).ThenInclude(p => p.Prodcut).FirstOrDefaultAsync(p => p.UserId == user.Id);
        
        int totalPrice = 0;
        foreach (var cartProduct in cart.ShoppingCartProducts)
        {
            totalPrice += cartProduct.Quantity * cartProduct.Prodcut.Price;
        }

        var createdStatus = await _context.OrderStatuses.Where(s => s.Status == "Created").FirstOrDefaultAsync();

        var order = new Order
        {
            OrderDate = DateOnly.FromDateTime(DateTime.Now),
            User = cart.User,
            Total = totalPrice,
            Status = createdStatus
        };

        _context.Add(order);
        
        var address = await _context.Addresses.FindAsync(addressId);

        var shippingInfo = new ShippingInfo
        {
            Street = address.Street,
            Apartment = address.Apartment,
            City = address.City,
            Country = address.Country,
            Province = address.Province,
            PostCode = address.PostCode,
            Order = order
        };

        _context.Add(shippingInfo);

        foreach (var product in cart.ShoppingCartProducts)
        {
            var orderProduct = new OrderProduct
            {
                Order = order,
                Product = product.Prodcut,
                Price = product.Prodcut.Price,
                Quantity = product.Quantity
            };

            _context.Add(orderProduct);
        }
        
        // Clear shopping cart
        _context.RemoveRange(cart.ShoppingCartProducts);

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Catalog");
    }

    private async Task CreateCartAsync(User user)
    {
        var cart = new ShoppingCart
        {
            User = user
        };
        await _context.AddAsync(cart);
        await _context.SaveChangesAsync();
    }
}