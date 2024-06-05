using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PCStore.Context;
using PCStore.Models;
using PCStore.Services;

namespace PCStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly PCStoreDBContext _context;

        public ProductController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: Product
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Index(int? categoryId)
        {
            IIncludableQueryable<Product, ProductCategory?> products;
            
            products = categoryId != null ? _context.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category) : _context.Products.Include(p => p.Category);

            return View(await products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.SpecsOptions)
                .ThenInclude(option => option.Spec)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Name,Price,Description,Stock")] Product product)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Price,Description,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> ImportProducts(IFormFile productsFile)
        {
            var importService = new ImportProductsService(_context);

            using var stream = productsFile.OpenReadStream();

            await importService.ImportFromStreamAsync(stream);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager, Admin")]
        public async Task<IActionResult> ExportProducts()
        {
            var exportService = new ExportProductsService(_context);

            var stream = new MemoryStream();

            await exportService.WriteToStreamAsync(stream);

            await stream.FlushAsync();
            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"products_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
    }
}
