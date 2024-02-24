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
    public class ShoppingCartProductsController : Controller
    {
        private readonly PCStoreDBContext _context;

        public ShoppingCartProductsController(PCStoreDBContext context)
        {
            _context = context;
        }

        // GET: ShoppingCartProducts
        public async Task<IActionResult> Index()
        {
            var pCStoreDBContext = _context.ShoppingCartProducts.Include(s => s.Cart).Include(s => s.Prodcut);
            return View(await pCStoreDBContext.ToListAsync());
        }

        // GET: ShoppingCartProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartProduct = await _context.ShoppingCartProducts
                .Include(s => s.Cart)
                .Include(s => s.Prodcut)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartProduct == null)
            {
                return NotFound();
            }

            return View(shoppingCartProduct);
        }

        // GET: ShoppingCartProducts/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id");
            ViewData["ProdcutId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: ShoppingCartProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CartId,ProdcutId,Quantity")] ShoppingCartProduct shoppingCartProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCartProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartProduct.CartId);
            ViewData["ProdcutId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartProduct.ProdcutId);
            return View(shoppingCartProduct);
        }

        // GET: ShoppingCartProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartProduct = await _context.ShoppingCartProducts.FindAsync(id);
            if (shoppingCartProduct == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartProduct.CartId);
            ViewData["ProdcutId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartProduct.ProdcutId);
            return View(shoppingCartProduct);
        }

        // POST: ShoppingCartProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CartId,ProdcutId,Quantity")] ShoppingCartProduct shoppingCartProduct)
        {
            if (id != shoppingCartProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCartProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartProductExists(shoppingCartProduct.Id))
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
            ViewData["CartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartProduct.CartId);
            ViewData["ProdcutId"] = new SelectList(_context.Products, "Id", "Id", shoppingCartProduct.ProdcutId);
            return View(shoppingCartProduct);
        }

        // GET: ShoppingCartProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartProduct = await _context.ShoppingCartProducts
                .Include(s => s.Cart)
                .Include(s => s.Prodcut)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartProduct == null)
            {
                return NotFound();
            }

            return View(shoppingCartProduct);
        }

        // POST: ShoppingCartProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCartProduct = await _context.ShoppingCartProducts.FindAsync(id);
            if (shoppingCartProduct != null)
            {
                _context.ShoppingCartProducts.Remove(shoppingCartProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartProductExists(int id)
        {
            return _context.ShoppingCartProducts.Any(e => e.Id == id);
        }
    }
}
