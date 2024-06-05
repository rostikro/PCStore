using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Services;

namespace PCStore.Controllers;

public class OrderController : Controller
{

    private readonly PCStoreDBContext _context;

    public OrderController(PCStoreDBContext context)
    {
        _context = context;
    }
    
    // GET
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.Include(o => o.User).Include(o => o.Status).ToListAsync();
        return View(orders);
    }

    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> Details(int? id)
    {
        var order = await _context.Orders.Where(o => o.Id == id).Include(o => o.OrderProducts).ThenInclude(p => p.Product)
            .ThenInclude(p => p.ProductImages).Include(o => o.ShippingInfos).FirstOrDefaultAsync();
        
        return View(order);
    }

    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> Accept(int? id)
    {
        var order = await _context.Orders.FindAsync(id);
        order.StatusId = 2;
        _context.Update(order);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    
    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> Decline(int? id)
    {
        var order = await _context.Orders.FindAsync(id);
        order.StatusId = 4;
        _context.Update(order);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Manager, Admin")]
    public async Task<IActionResult> GetReceipt(int? id)
    {
        var receiptService = new OrderReceiptService();

        var stream = new MemoryStream();

        receiptService.WriteToStreamAsync(stream, await _context.Orders.Where(o => o.Id == id).Include(o => o.OrderProducts).ThenInclude(o => o.Product).FirstOrDefaultAsync());

        await stream.FlushAsync();
        stream.Position = 0;

        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            FileDownloadName = $"Order_{id}_receipt.docx"
        };
    }
}