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
    public class SpecOptionsController : Controller
    {
        private readonly PCStoreDBContext _context;

        public SpecOptionsController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: SpecsOptions
        public async Task<IActionResult> Index()
        {
            var pCStoreDBContext = _context.SpecsOptions.Include(s => s.Product).Include(s => s.Spec);
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: SpecsOptions/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: SpecsOptions/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Id");
            return View();
        }

        // POST: SpecsOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecId,ProductId,Value")] SpecsOption specsOption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specsOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Id", specsOption.SpecId);
            return View(specsOption);
        }

        // GET: SpecsOptions/Edit/5
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Id", specsOption.SpecId);
            return View(specsOption);
        }

        // POST: SpecsOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", specsOption.ProductId);
            ViewData["SpecId"] = new SelectList(_context.Specs, "Id", "Id", specsOption.SpecId);
            return View(specsOption);
        }

        // GET: SpecsOptions/Delete/5
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specsOption = await _context.SpecsOptions.FindAsync(id);
            if (specsOption != null)
            {
                _context.SpecsOptions.Remove(specsOption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecsOptionExists(int id)
        {
            return _context.SpecsOptions.Any(e => e.Id == id);
        }
    }
}
