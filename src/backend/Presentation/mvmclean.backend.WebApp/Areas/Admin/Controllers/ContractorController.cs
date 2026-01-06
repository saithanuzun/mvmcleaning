using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Contractor")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class ContractorController : BaseAdminController
{
    public ContractorController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("all")]
    public async Task<IActionResult> AllContractors()
    {
        try
        {
            var response = await _mediator.Send(new GetAllContractorsRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading contractors: {ex.Message}";
            return View(new GetAllContractorsResponse());
        }
    }

    [Route("details/{contractorId}")]
    public async Task<IActionResult> Details(string contractorId)
    {
        if (string.IsNullOrEmpty(contractorId))
            return RedirectToAction("AllContractors");

        try
        {
            var response = await _mediator.Send(new GetContractorByIdRequest { Id = contractorId });
            ViewBag.Title = $"Contractor - {response.FullName}";
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("AllContractors");
        }
    }

    [Route("services/{contractorId}")]
    public async Task<IActionResult> Services(string contractorId)
    {
        if (string.IsNullOrEmpty(contractorId))
            return RedirectToAction("AllContractors");

        try
        {
            var response = await _mediator.Send(new GetContractorByIdRequest { Id = contractorId });
            ViewBag.Title = $"Services - {response.FullName}";
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("AllContractors");
        }
    }

    [Route("coverage/{contractorId}")]
    public async Task<IActionResult> Coverage(string contractorId)
    {
        if (string.IsNullOrEmpty(contractorId))
            return RedirectToAction("AllContractors");

        try
        {
            var response = await _mediator.Send(new GetContractorCoveragesRequest { ContractorId = contractorId });
            ViewBag.Title = $"Coverage Areas - {response.ContractorId}";
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("AllContractors");
        }
    }

    [Route("availability/{contractorId}")]
    public async Task<IActionResult> Availability(string contractorId)
    {
        if (string.IsNullOrEmpty(contractorId))
            return RedirectToAction("AllContractors");

        try
        {
            var response = await _mediator.Send(new GetContractorByIdRequest { Id = contractorId });
            ViewBag.Title = $"Availability - {response.FullName}";
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("AllContractors");
        }
    }
}