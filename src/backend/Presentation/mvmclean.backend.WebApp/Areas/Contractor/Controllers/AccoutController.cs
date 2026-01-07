using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor")] 
public class AccountController : BaseContractorController
{
    public AccountController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("Login")]
    [Route("Login")]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var request = new LoginRequest
        {
            Usename = username,
            Password = password,
        };

        try
        {
            var response = await _mediator.Send(request);

            if (response.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.Username),
                    new Claim(ClaimTypes.NameIdentifier, response.ContractorId.ToString()),
                    new Claim(ClaimTypes.Role, "ContractorCookie")
                };

                var identity = new ClaimsIdentity(claims, "ContractorCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("ContractorCookie", principal);

                return RedirectToAction("Index", "Home", new { area = "Contractor" });
            }

            ViewBag.Error = response;
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Error = "An unexpected error occurred.";
            return View();
        }
    }

    

    [HttpGet("Logout")]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("ContractorCookie");
        return RedirectToAction("Login");
    }

    [HttpGet]
    [Route("Register")]
    public IActionResult Register()
    {
        return View(); 
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Register")]
    public async Task<IActionResult> Register(CreateContractorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var response = await _mediator.Send(request);

        return RedirectToAction("index", "Home", new { area = "Contractor" });
    }




}