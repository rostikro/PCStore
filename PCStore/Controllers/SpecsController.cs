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
    public class SpecsController : Controller
    {
        private readonly PCStoreDBContext _context;

        public SpecsController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: Specs
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Index(int categoryId)
        {
            var pCStoreDBContext = _context.Specs
                .Where(s => s.CategoryId == categoryId)
                .Include(s => s.Category);

            var currentCategory = await _context.ProductCategories.Where(p => p.Id == categoryId).FirstOrDefaultAsync();
            ViewData["CategoryId"] = currentCategory?.Id;
            ViewData["CategoryName"] = currentCategory?.Name;
            
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: Specs/Create
        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Create(int categoryId)
        {
            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", categoryId);
            return View();
        }

        // POST: Specs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Name")] Spec spec)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spec);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { categoryId = spec.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", spec.CategoryId);
            return View(spec);
        }

        // GET: Specs/Edit/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spec = await _context.Specs.FindAsync(id);
            if (spec == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", spec.CategoryId);
            return View(spec);
        }

        // POST: Specs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name")] Spec spec)
        {
            
            if (id != spec.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecExists(spec.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { categoryId = spec.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", spec.CategoryId);
            return View(spec);
        }

        // GET: Specs/Delete/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spec = await _context.Specs
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spec == null)
            {
                return NotFound();
            }

            return View(spec);
        }

        // POST: Specs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spec = await _context.Specs.FindAsync(id);
            if (spec != null)
            {
                _context.Specs.Remove(spec);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { categoryId = spec?.CategoryId });
        }

        private bool SpecExists(int id)
        {
            return _context.Specs.Any(e => e.Id == id);
        }
    }
}
