using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Services;

public class ExportProductsService
{
    private const string productsWorksheetName = "Products";
    private const string productImagesWorksheetName = "Products Images";
    private const string specsOptionsWorksheetName = "Specs Options";

    private static readonly IReadOnlyList<string> ProductsHeaderNames = new string[]
    {
        "Назва",
        "Ціна",
        "Опис",
        "Кількість",
        "Категорія",
    };

    private static readonly IReadOnlyList<string> ProductsImagesHeaderNames = new string[]
    {
        "Url",
        "Продукт"
    };

    private static readonly IReadOnlyList<string> SpecsOptionseHeaderNames = new string[]
    {
        "Значення",
        "Продукт",
        "Spec"
    };
    
    private readonly PCStoreDBContext context;

    public ExportProductsService(PCStoreDBContext context)
    {
        this.context = context;
    }

    public async Task WriteToStreamAsync(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Input stream is not writable");
        }

        var products = await context.Products.Include(p => p.Category).ToListAsync();
        var productImages = await context.ProductImages.Include(p => p.Product).ToListAsync();
        var specsOptions = await context.SpecsOptions.Include(option => option.Product).Include(option => option.Spec).ToListAsync();

        using var workbook = new XLWorkbook();
        
        WriteProducts(workbook.Worksheets.Add(productsWorksheetName), products);
        WriteProductImages(workbook.Worksheets.Add(productImagesWorksheetName), productImages);
        WriteSpecsOptions(workbook.Worksheets.Add(specsOptionsWorksheetName), specsOptions);
        
        workbook.SaveAs(stream);
    }

    private static void WriteProducts(IXLWorksheet worksheet, ICollection<Product> products)
    {
        WriteHeader(worksheet, ProductsHeaderNames);

        int i = 2;
        foreach (var product in products)
        {
            var j = 1;
            worksheet.Cell(i, j++).Value = product.Name;
            worksheet.Cell(i, j++).Value = product.Price;
            worksheet.Cell(i, j++).Value = product.Description;
            worksheet.Cell(i, j++).Value = product.Stock;
            worksheet.Cell(i, j++).Value = product.Category.Name;
            i++;
        }
    }

    private static void WriteProductImages(IXLWorksheet worksheet, ICollection<ProductImage> productImages)
    {
        WriteHeader(worksheet, ProductsImagesHeaderNames);

        int i = 2;
        foreach (var productImage in productImages)
        {
            var j = 1;
            worksheet.Cell(i, j++).Value = productImage.Url;
            worksheet.Cell(i, j++).Value = productImage.Product.Name;
            i++;
        }
    }

    private static void WriteSpecsOptions(IXLWorksheet worksheet, ICollection<SpecsOption> specsOptions)
    {
        WriteHeader(worksheet, SpecsOptionseHeaderNames);

        int i = 2;
        foreach (var specsOption in specsOptions)
        {
            var j = 1;
            worksheet.Cell(i, j++).Value = specsOption.Value;
            worksheet.Cell(i, j++).Value = specsOption.Product.Name;
            worksheet.Cell(i, j++).Value = specsOption.Spec.Name;
            i++;
        }
    }

    private static void WriteHeader(IXLWorksheet worksheet, IReadOnlyList<string> headers)
    {
        for (int i = 0; i < headers.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        worksheet.Row(1).Style.Font.Bold = true;
    }
}