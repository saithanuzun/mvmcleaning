using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.SeoPage.Queries;

namespace mvmclean.backend.WebApp.Controllers;

public class HomeController : BaseController
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("/")]
    public IActionResult Index() => View();
    
    
    [Route("/home")]
    public IActionResult Home() => RedirectPermanent("/");

    
    [Route("/book-now")]
    public IActionResult BookNow() => RedirectPermanent("/shop");
    
    [Route("/get-a-quote")]
    public IActionResult GetAQuote() => RedirectPermanent("/shop");
    
    [Route("/packages")]
    public IActionResult Packages() => RedirectPermanent("/shop");
    
    [Route("/gallery")]
    public IActionResult Gallery() => View();
    
    
    [Route("/about-us")]
    public IActionResult About() => View();
    
    
    [Route("/faqs")]
    public IActionResult FAQs() => View();
    
    
    [Route("/terms-and-conditions")]
    public IActionResult TermsAndConditions() => View();

    [Route("/areas-we-serve")]
    public async Task<IActionResult> AreasWeServe()
    {
        var response = await _mediator.Send(new GetAllSeoPagesRequest());
        return View(response);
    }
}