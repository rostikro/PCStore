using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using PCStore.Context;
using PCStore.Models;

namespace PCStore.Controllers;

public class CabinetController : Controller
{
 
    private readonly PCStoreDBContext _context;
    private readonly UserManager<User> _userManager;
    
    public CabinetController(PCStoreDBContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> PersonalInformation()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> PersonalInformation(string username)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        user.UserName = username;
        
        if (ModelState.IsValid)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("PersonalInformation");
    }

    [HttpGet]
    public async Task<IActionResult> Addresses()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var addresses = _context.Addresses.Where(a => a.UserId == user.Id);
        return View(addresses);
    }

    [HttpGet]
    public IActionResult AddAddress()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([Bind("Id,Street,Apartment,City,Province,PostCode,Country")] Address address)
    {
        ModelState.Remove("UserId");
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            address.User = user;
            _context.Add(address);
            await _context.SaveChangesAsync();
            return RedirectToAction("Addresses");
        }
        return View(address);
    }

    [HttpGet]
    public async Task<IActionResult> EditAddress(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var address = await _context.Addresses.FindAsync(id);
        if (address == null)
        {
            return NotFound();
        }
        
        return View(address);
    }

    [HttpPost]
    public async Task<IActionResult> EditAddress(int? id,
        [Bind("Id,Street,Apartment,City,Province,PostCode,Country")] Address address)
    {
        if (id != address.Id)
        {
            return NotFound();
        }

        ModelState.Remove("UserId");
        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                address.User = user;
                address.UserId = user.Id;
                _context.Update(address);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Addresses.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction("Addresses");
        }

        return View(address);
    }

    public async Task<IActionResult> DeleteAddress(int? id)
    {
        var address = await _context.Addresses.FindAsync(id);
        if (address != null)
        {
            _context.Remove(address);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Addresses");
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return View();
    }
}