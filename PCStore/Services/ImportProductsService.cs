using System.Diagnostics;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Services;

public class ImportProductsService
{
    private readonly PCStoreDBContext context;

    public ImportProductsService(PCStoreDBContext context)
    {
        this.context = context;
    }

    public async Task ImportFromStreamAsync(Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream is not readable", nameof(stream));
        }

        using var workBook = new XLWorkbook(stream);
        var worksheets = workBook.Worksheets.ToList();
        if (worksheets.Count != 3)
        {
            return;
        }
        
        // Add products
        foreach (var row in worksheets[0].RowsUsed().Skip(1))
        {
            context.Add(await AddProductAsync(row));
        }
        await context.SaveChangesAsync();
        
        // Add product images
        foreach (var row in worksheets[1].RowsUsed().Skip(1))
        {
            context.Add(await AddProductImageAsync(row));
        }
        await context.SaveChangesAsync();
        
        // Add product specs
        foreach (var row in worksheets[2].RowsUsed().Skip(1))
        {
            context.Add(await AddSpecsOptionAsync(row));
        }
        await context.SaveChangesAsync();

        /*await context.SaveChangesAsync();*/
    }

    private async Task<Product> AddProductAsync(IXLRow row)
    {
        var productName = GetProductName(row);

        var product = await context.Products.FirstOrDefaultAsync(p => p.Name == productName);

        if (product == null)
        {
            product = new Product
            {
                Name = productName,
                Price = GetProductPrice(row),
                Description = GetProductDescription(row),
                Stock = GetProductStock(row),
                CategoryId = await GetProductCategoryId(row)
            };
            /*await context.Products.AddAsync(product);*/
        }

        return product;
    }

    private async Task<ProductImage> AddProductImageAsync(IXLRow row)
    {
        return new ProductImage
        {
            Url = GetProductImageUrl(row),
            ProductId = await GetProductId(row),
        };
    }

    private async Task<SpecsOption> AddSpecsOptionAsync(IXLRow row)
    {
        return new SpecsOption
        {
            Value = GetSpecsOptionValue(row),
            ProductId = await GetProductId(row),
            SpecId = await GetSpecId(row),
        };
    }

    private static string GetProductName(IXLRow row)
    {
        return row.Cell(1).GetValue<string>();
    }

    private static int GetProductPrice(IXLRow row)
    {
        return row.Cell(2).GetValue<int>();
    }

    private static string GetProductDescription(IXLRow row)
    {
        return row.Cell(3).GetValue<string>();
    }
    
    private static int GetProductStock(IXLRow row)
    {
        return row.Cell(4).GetValue<int>();
    }

    private async Task<int> GetProductCategoryId(IXLRow row)
    {
        var categoryName = row.Cell(5).GetValue<string>();
        var category = await context.ProductCategories.FirstOrDefaultAsync(p => p.Name == categoryName);
        if (category == null)
        {
            category = new ProductCategory
            {
                Name = categoryName
            };
            context.Add(category);
        }

        return category.Id;
    }

    private static string GetProductImageUrl(IXLRow row)
    {
        return row.Cell(1).GetValue<string>();
    }

    private async Task<int> GetProductId(IXLRow row)
    {
        var productName = row.Cell(2).GetValue<string>();
        var product = await context.Products.FirstOrDefaultAsync(p => p.Name == productName);
        return product.Id;
    }

    private static string GetSpecsOptionValue(IXLRow row)
    {
        return row.Cell(1).GetValue<string>();
    }

    private async Task<int> GetSpecId(IXLRow row)
    {
        var specName = row.Cell(3).GetValue<string>();
        var spec = await context.Specs.FirstOrDefaultAsync(s => s.Name == specName);
        return spec.Id;
    }
}