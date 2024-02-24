using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Controllers
{
    public class ShippingInfoController : Controller
    {
        private readonly PCStoreDBContext _context;

        public ShippingInfoController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: ShippingInfo
        public async Task<IActionResult> Index()
        {
            var pCStoreDBContext = _context.ShippingInfos.Include(s => s.Order);
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: ShippingInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingInfo = await _context.ShippingInfos
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingInfo == null)
            {
                return NotFound();
            }

            return View(shippingInfo);
        }

        // GET: ShippingInfo/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            return View();
        }

        // POST: ShippingInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,Street,Apartment,City,Province,PostCode,Country")] ShippingInfo shippingInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shippingInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", shippingInfo.OrderId);
            return View(shippingInfo);
        }

        // GET: ShippingInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingInfo = await _context.ShippingInfos.FindAsync(id);
            if (shippingInfo == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", shippingInfo.OrderId);
            return View(shippingInfo);
        }

        // POST: ShippingInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,Street,Apartment,City,Province,PostCode,Country")] ShippingInfo shippingInfo)
        {
            if (id != shippingInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingInfoExists(shippingInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", shippingInfo.OrderId);
            return View(shippingInfo);
        }

        // GET: ShippingInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingInfo = await _context.ShippingInfos
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shippingInfo == null)
            {
                return NotFound();
            }

            return View(shippingInfo);
        }

        // POST: ShippingInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shippingInfo = await _context.ShippingInfos.FindAsync(id);
            if (shippingInfo != null)
            {
                _context.ShippingInfos.Remove(shippingInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingInfoExists(int id)
        {
            return _context.ShippingInfos.Any(e => e.Id == id);
        }
    }
}
