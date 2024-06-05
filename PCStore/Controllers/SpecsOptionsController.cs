using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Controllers
{
    public class SpecsOptionsController : Controller
    {
        private readonly PCStoreDBContext _context;

        public SpecsOptionsController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: SpecsOptions
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Index(int productId)
        {
            var pCStoreDBContext = _context.SpecsOptions
                .Where(specOpt => specOpt.ProductId == productId)
                .Include(s => s.Product).Include(s => s.Spec);
            
            var currentProduct = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
            ViewData["ProductId"] = currentProduct?.Id;
            ViewData["ProductName"] = currentProduct?.Name;
            
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: SpecsOptions/Create
        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Create(int productId)
        {
            ViewData["CurrentProductId"] = productId;
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Name");
            return View();
        }

        // POST: SpecsOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecId,ProductId,Value")] SpecsOption specsOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specsOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { productId = specsOption.ProductId });
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Name", specsOption.SpecId);
            return View(specsOption);
        }

        // GET: SpecsOptions/Edit/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specsOption = await _context.SpecsOptions.FindAsync(id);
            if (specsOption == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Name", specsOption.SpecId);
            return View(specsOption);
        }

        // POST: SpecsOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpecId,ProductId,Value")] SpecsOption specsOption)
        {
            if (id != specsOption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specsOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecsOptionExists(specsOption.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { productId = specsOption.ProductId });
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Name", specsOption.SpecId);
            return View(specsOption);
        }

        // GET: SpecsOptions/Delete/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specsOption = await _context.SpecsOptions
                .Include(s => s.Product)
                .Include(s => s.Spec)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specsOption == null)
            {
                return NotFound();
            }

            return View(specsOption);
        }

        // POST: SpecsOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specsOption = await _context.SpecsOptions.FindAsync(id);
            if (specsOption != null)
            {
                _context.SpecsOptions.Remove(specsOption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { productId = specsOption?.ProductId });
        }

        private bool SpecsOptionExists(int id)
        {
            return _context.SpecsOptions.Any(e => e.Id == id);
        }
    }
}
