using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace mvmclean.backend.WebApp.Controllers;

public class ServicesController : BaseController
{
    
    [HttpGet]
    [Route("/services")]
    public IActionResult Index()
    {
        return RedirectPermanent("/");
    }


    [HttpGet]
    [Route("/services/jet-washing")]
    public IActionResult JetWashing()
    {
        return View();
    }
    
    [HttpGet]
    [Route("/services/carpet-cleaning")]
    public IActionResult CarpetCleaning()
    {
        return RedirectPermanent("/");
    }
    
    [HttpGet]
    [Route("/services/sofa-cleaning")]
    public IActionResult SofaCleaning()
    {
        return RedirectPermanent("/");
    }
    
    [HttpGet]
    [Route("/services/stain-removal")]
    public IActionResult StainRemoval()
    {
        return RedirectPermanent("/");
    }
    
    [HttpGet]
    [Route("/services/mattress-cleaning")]
    public IActionResult MattressCleaning()
    {
        return RedirectPermanent("/");
    }
    
    [HttpGet]
    [Route("/services/upholstery-cleaning")]
    public IActionResult UpholsteryCleaning()
    {
        return RedirectPermanent("/");
    }

    
  
   
}
