using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/coverage")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class CoverageController : BaseContractorController
{
    public CoverageController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Service Coverage Areas";
        return View(response);
    }

    [Route("list")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            var response = await _mediator.Send(new GetContractorCoveragesRequest { ContractorId = ContractorId.ToString() });
            ViewBag.Title = "Coverage Areas";
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [Route("add")]
    [HttpPost]
    public async Task<IActionResult> Add(string postcode)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        if (string.IsNullOrWhiteSpace(postcode))
        {
            TempData["Error"] = "Postcode is required";
            return RedirectToAction("Index");
        }

        try
        {
            var request = new CreateContractorCoverageRequest
            {
                ContractorId = ContractorId.ToString(),
                Postcode = postcode
            };

            await _mediator.Send(request);
            TempData["Success"] = "Coverage area has been added successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}
