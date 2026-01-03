using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor")] 
[Authorize(AuthenticationSchemes = "ContractorCookie")] 
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
    
    [Route("Services")]
    public IActionResult Services()
    {
        return View();
    }
    
    [Route("Profile")]
    public IActionResult Profile()
    {
        return View();
    }
    
    [Route("Workingdays")]
    public IActionResult Workingdays()
    {
        return View();
    }
    
    [Route("Availability")]
    public IActionResult Availability()
    {
        return View();
    }
    
    
}