using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class HomeController : Controller
{
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
    
    [Route("Bookings")]
    public IActionResult Bookings()
    {
        return View();
    }
    
    [Route("CreateService")]
    public IActionResult CreateService()
    {
        return View();
    }
    
    [Route("AllServices")]
    
    public IActionResult AllServices()
    {
        return View();
    }
    
}