using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Encryptor;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin")]
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
        var hashedUsername = "4FC393062A8C1F95F0C8AB023F4F25D7"; // todo
        var hashedPassword = "A9A624B3C4D4BE9146618BD94BEFF8E0";
        
        var _hashedUsernameLogin = PasswordEncryptor.Encrypt(username);
        var _hashedPasswordLogin = PasswordEncryptor.Encrypt(password);
        
        if (_hashedUsernameLogin == hashedUsername && _hashedPasswordLogin == hashedPassword)
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