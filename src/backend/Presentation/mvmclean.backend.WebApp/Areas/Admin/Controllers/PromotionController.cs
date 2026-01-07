using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Promotion;
using mvmclean.backend.Application.Features.Promotion.Commands;
using mvmclean.backend.Application.Features.Promotion.Queries;
using mvmclean.backend.Application.Features.Promotion.Commands;
using mvmclean.backend.Application.Features.Promotion.Queries;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

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

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePromotionRequest());
    }

    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePromotionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var response = await _mediator.Send(request);
            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(AllPromotions));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating promotion: {ex.Message}";
            return View(request);
        }
    }

    [Route("activate/{promotionId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid promotionId)
    {
        try
        {
            var response = await _mediator.Send(new UpdatePromotionStatusRequest
            {
                PromotionId = promotionId,
                IsActive = true
            });

            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Details), new { promotionId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error activating promotion: {ex.Message}";
            return RedirectToAction(nameof(AllPromotions));
        }
    }

    [Route("deactivate/{promotionId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid promotionId)
    {
        try
        {
            var response = await _mediator.Send(new UpdatePromotionStatusRequest
            {
                PromotionId = promotionId,
                IsActive = false
            });

            TempData["Success"] = response.Message;
            return RedirectToAction(nameof(Details), new { promotionId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deactivating promotion: {ex.Message}";
            return RedirectToAction(nameof(AllPromotions));
        }
    }
}