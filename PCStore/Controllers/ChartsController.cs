using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;

namespace PCStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private record SalesStatsItem(string Date, int Count);

    private record IncomingStatsItem(string Date, int Incoming);

    private record CategoriesSalesStatsItem(string Category, int Count);

    private readonly PCStoreDBContext context;

    public ChartsController(PCStoreDBContext context)
    {
        this.context = context;
    }

    [HttpGet("salesStats")]
    public async Task<JsonResult> SalesStatsAsync()
    {
        var orders = await context.Orders.GroupBy(order => order.OrderDate)
            .Select(group => new SalesStatsItem(group.Key.ToString(), group.Count())).ToListAsync();

        return new JsonResult(orders);
    }

    [HttpGet("incomingStats")]
    public async Task<JsonResult> IncomingStatsAsync()
    {
        var orders = await context.Orders.GroupBy(order => order.OrderDate)
            .Select(group => new IncomingStatsItem(group.Key.ToString(), group.Sum(o => o.Total))).ToListAsync();

        return new JsonResult(orders);
    }

    [HttpGet("categoriesSalesStats")]
    public async Task<JsonResult> CategoriesSalesStatsAsync()
    {
        var categories = await context.OrderProducts.GroupBy(product => product.Product.Category.Name)
            .Select(group => new CategoriesSalesStatsItem(group.Key.ToString(), group.Count())).ToListAsync();

        return new JsonResult(categories);
    }
}