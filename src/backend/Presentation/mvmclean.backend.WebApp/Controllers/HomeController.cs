using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index() => View();
    
    [Route("contact")]
    public IActionResult Contact() => View();
    
    
    [Route("gallery")]
    public IActionResult Gallery() => View();
    
    
    [Route("about-us")]
    public IActionResult About() => View();
    
    
    [Route("faqs")]
    public IActionResult FAQs() => View();
    
    
    [Route("terms-and-conditions")]
    public IActionResult TermsAndConditions() => View();
}