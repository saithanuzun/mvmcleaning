using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/profile")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class ProfileController : BaseContractorController
{
    public ProfileController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "My Profile";
        return View(response);
    }

    [Route("edit")]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Edit Profile";
        return View(response);
    }
}
