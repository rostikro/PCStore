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
    public class ProductImagesController : Controller
    {
        private readonly PCStoreDBContext _context;

        public ProductImagesController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: ProductImages
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Index(int productId)
        {
            var pCStoreDBContext = _context.ProductImages
                .Where(img => img.ProductId == productId)
                .Include(p => p.Product);

            var currentProduct = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
            ViewData["ProductId"] = currentProduct?.Id;
            ViewData["ProductName"] = currentProduct?.Name;
            
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: ProductImages/Create
        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Create(int productId)
        {
            ViewData["CurrentProductId"] = productId;
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productId);
            return View();
        }

        // POST: ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Url")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { productId = productImage.ProductId });
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: ProductImages/Edit/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Url")] ProductImage productImage)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { productId = productImage.ProductId });
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { productId = productImage?.ProductId });
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }
    }
}
