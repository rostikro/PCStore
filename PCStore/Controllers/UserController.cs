using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using PCStore.Models;
using PCStore.ViewModels;

namespace PCStore.Controllers;

public class UserController : Controller
{

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;
    
    public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = new User { Email = viewModel.Email, UserName = viewModel.Email };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                /*var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                /*if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }*/

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "User",
                    values: new { userId, code },
                    "https");

                await _emailSender.SendEmailAsync(user.Email, "Підтвердіть свою пошту",
                    $"Будь-ласка підтвердіть свою пошту <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснувши тут</a>");
                
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("ActionStatus", new { title = "Підтвердження пошти", status = "Перевірте свою пошту."});
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult ActionStatus(string title, string status)
    {
        ViewData["Title"] = title;
        ViewData["Status"] = status;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Catalog");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction("ActionStatus", new { title = "Підтвердження пошти", status = "Нема такого користувача." });
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return RedirectToAction("ActionStatus",
            new
            {
                title = "Підтвердження пошти",
                status = result.Succeeded ? "Дякую за підтвердження пошти." : "Сталася помилка."
            });
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.SaveCookies, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Catalog");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ім'я користувача або пароль неправильний.");
            }
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Catalog");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user == null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ResetPassword",
                "User",
                values: new { email = user.Email, code },
                "https");

            await _emailSender.SendEmailAsync(viewModel.Email, "Скидання паролю",
                $"Скиньте свій пароль <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснувши тут</a>.");
            
            return RedirectToAction("ActionStatus", new { title = "Скидання паролю", status = "Перевірте свою пошту."});
        }

        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string code)
    {
        if (email == null || code == null)
        {
            return RedirectToAction("Login");
        }
        
        return View(new ResetPasswordViewModel
        {
            Email = email,
            Code = code
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(new ResetPasswordViewModel
            {
                Email = viewModel.Email,
                Code = viewModel.Code
            });
        }

        var user = await _userManager.FindByEmailAsync(viewModel.Email);
        if (user == null)
        {
            return RedirectToAction("Login");
        }
        
        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(viewModel.Code));

        var result = await _userManager.ResetPasswordAsync(user, code, viewModel.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("ResetPasswordSuccess");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(new ResetPasswordViewModel
        {
            Email = viewModel.Email,
            Code = viewModel.Code
        });
    }

    [HttpGet]
    public IActionResult ResetPasswordSuccess()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EmployeesList()
    {
        var managers = await _userManager.GetUsersInRoleAsync("Manager");

        var employees = managers.Select(user => new EmployeeListViewModel
        {
            Email = user.Email,
            Role = "Manager"
        });
        
        return View(employees);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult AddEmployee()
    {
        ViewData["Roles"] = new SelectList(new[] { "Manager" }, "Manager");
        return View(new EmployeeListViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddEmployee(EmployeeListViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, viewModel.Role);
                return RedirectToAction("EmployeesList");
            }
            ModelState.AddModelError(string.Empty, "Пошта не вірна.");
        }

        ViewData["Roles"] = new SelectList(new[] { "Admin", "Manager" }, "Manager");
        return View(new EmployeeListViewModel());
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveEmployee(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Error");
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(user, "Manager");
        }
        return RedirectToAction("EmployeesList");
    }
}