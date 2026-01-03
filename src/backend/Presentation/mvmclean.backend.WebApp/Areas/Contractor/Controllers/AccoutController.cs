using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor")]
public class AccountController : Controller
{
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (username == "saithanuzun" && password == "1234")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "AdminCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AdminCookie", principal);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        ViewBag.Error = "Invalid credentials";
        return View();
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AdminCookie");
        return RedirectToAction("Login");
    }
}