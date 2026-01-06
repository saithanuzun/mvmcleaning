using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Promotion;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Promotion")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class PromotionController : BaseAdminController
{
    public PromotionController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllPromotions();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllPromotions()
    {
        try
        {
            var response = await _mediator.Send(new GetAllPromotionsRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading promotions: {ex.Message}";
            return View(new GetAllPromotionsResponse());
        }
    }

    [Route("details/{promotionId}")]
    [HttpGet]
    public async Task<IActionResult> Details(Guid promotionId)
    {
        try
        {
            var response = await _mediator.Send(new GetPromotionByIdRequest { PromotionId = promotionId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Promotion not found";
            return RedirectToAction(nameof(AllPromotions));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading promotion: {ex.Message}";
            return RedirectToAction(nameof(AllPromotions));
        }
    }
}