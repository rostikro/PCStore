using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PCStore.Controllers;

public class AdminPanelController : Controller
{
    [Authorize(Roles = "Manager, Admin")]
    public IActionResult Index()
    {
        return View();
    }
}