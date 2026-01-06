using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.SeoPage;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/SeoPage")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class SeoPageController : BaseAdminController
{
    public SeoPageController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllPages();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllPages()
    {
        try
        {
            var response = await _mediator.Send(new GetAllSeoPagesRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading SEO pages: {ex.Message}";
            return View(new GetAllSeoPagesResponse());
        }
    }

    [Route("details/{pageId}")]
    [HttpGet]
    public async Task<IActionResult> Details(Guid pageId)
    {
        try
        {
            var response = await _mediator.Send(new GetSeoPageByIdRequest { PageId = pageId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "SEO Page not found";
            return RedirectToAction(nameof(AllPages));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading SEO page: {ex.Message}";
            return RedirectToAction(nameof(AllPages));
        }
    }
}