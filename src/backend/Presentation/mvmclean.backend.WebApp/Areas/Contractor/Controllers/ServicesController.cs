using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/services")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class ServicesController : BaseContractorController
{
    public ServicesController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            // Get all available services
            var allServices = await _mediator.Send(new GetAllServicesRequest());

            // Get contractor's current services
            var contractor = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
            ViewBag.ContractorServices = contractor.Services?.Select(s => s.ServiceId).ToList() ?? new List<Guid>();

            return View(allServices);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading services: {ex.Message}";
            return View(new List<GetAllServicesResponse>());
        }
    }

    [Route("details/{serviceId}")]
    [HttpGet]
    public async Task<IActionResult> Details(Guid serviceId)
    {
        try
        {
            var response = await _mediator.Send(new GetServiceByIdRequest { ServiceId = serviceId });

            if (response == null)
            {
                TempData["Error"] = "Service not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading service details: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [Route("add/{serviceId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(Guid serviceId)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            var response = await _mediator.Send(new AddServiceToContractorRequest
            {
                ContractorId = ContractorId.ToString(),
                ServiceId = serviceId.ToString()
            });

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error adding service: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [Route("remove/{serviceId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveService(Guid serviceId)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            var response = await _mediator.Send(new RemoveServiceFromContractorRequest
            {
                ContractorId = ContractorId.ToString(),
                ServiceId = serviceId.ToString()
            });

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error removing service: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
