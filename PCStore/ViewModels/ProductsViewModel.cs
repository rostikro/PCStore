using PCStore.Models;

namespace PCStore.ViewModels;

public class ProductsViewModel
{
    public IEnumerable<ProductCategory> Categories { get; set; }
    
    public IEnumerable<Product> Products { get; set; }
}