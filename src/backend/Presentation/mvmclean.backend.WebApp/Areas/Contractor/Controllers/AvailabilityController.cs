using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/availability")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class AvailabilityController : BaseContractorController
{
    public AvailabilityController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "My Availability";
        return View(response);
    }

    [Route("working-days")]
    [HttpGet]
    public async Task<IActionResult> WorkingDays()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Working Days";
        return View(response);
    }

    [Route("unavailability")]
    [HttpGet]
    public async Task<IActionResult> Unavailability()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Unavailability";
        return View(response);
    }

    [Route("unavailability/add")]
    [HttpPost]
    public async Task<IActionResult> AddUnavailability(DateTime startTime, DateTime endTime)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        if (startTime >= endTime)
        {
            TempData["Error"] = "Start time must be before end time";
            return RedirectToAction("Unavailability");
        }

        try
        {
            var request = new CreateUnavailabilityRequest
            {
                ContractorId = ContractorId.ToString(),
                Startime = startTime,
                EndTime = endTime
            };

            var response = await _mediator.Send(request);
            TempData["Success"] = "Unavailability period has been added successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Unavailability");
    }
}
