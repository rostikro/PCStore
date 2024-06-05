using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PCStore.Context;
using PCStore.Models;
using PCStore.ViewModels;

namespace PCStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly PCStoreDBContext _context;

        public CatalogController(PCStoreDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            IIncludableQueryable<Product, IEnumerable<ProductImage>> products;
            
            if (categoryId.HasValue)
            {
                products = _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages);
            }
            else
            {
                products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages);
            }
            
            var productsViewModel = new ProductsViewModel
            {
                Products = await products.ToListAsync(),
                Categories = await _context.ProductCategories.ToListAsync()
            };

            return View(productsViewModel);
        }

        public async Task<IActionResult> SearchProduct(string product_name)
        {
            var products = _context.Products.Where(p => p.Name.Contains(product_name))
                .Include(p => p.Category)
                .Include(p => p.ProductImages);

            var productsViewModel = new ProductsViewModel
            {
                Products = await products.ToListAsync(),
                Categories = await _context.ProductCategories.ToListAsync()
            };

            ViewBag.SearchBarValue = product_name;
            
            return View("Index", productsViewModel);
        }

        /*public async Task<IActionResult> ProductDetails(int? productId)
        {
            if (!productId.HasValue)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == productId);

            if (product == null)
                return NotFound();

            return View(product);
        }*/
        
        //
        // // GET: Products/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var product = await _context.Products
        //         .Include(p => p.Category)
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (product == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(product);
        // }
        //
        // // GET: Products/Create
        // public IActionResult Create()
        // {
        //     ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id");
        //     return View();
        // }
        //
        // // POST: Products/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,CategoryId,Name,Price,Description,Stock")] Product product)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(product);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.CategoryId);
        //     return View(product);
        // }
        //
        // // GET: Products/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var product = await _context.Products.FindAsync(id);
        //     if (product == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.CategoryId);
        //     return View(product);
        // }
        //
        // // POST: Products/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Price,Description,Stock")] Product product)
        // {
        //     if (id != product.Id)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(product);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!ProductExists(product.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.CategoryId);
        //     return View(product);
        // }
        //
        // // GET: Products/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var product = await _context.Products
        //         .Include(p => p.Category)
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (product == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(product);
        // }
        //
        // // POST: Products/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var product = await _context.Products.FindAsync(id);
        //     if (product != null)
        //     {
        //         _context.Products.Remove(product);
        //     }
        //
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
        //
        // private bool ProductExists(int id)
        // {
        //     return _context.Products.Any(e => e.Id == id);
        // }
    }
}
