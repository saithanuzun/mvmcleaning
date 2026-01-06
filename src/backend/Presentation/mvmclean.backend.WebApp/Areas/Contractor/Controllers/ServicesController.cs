using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;

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

        var response = await _mediator.Send(new GetAllServicesRequest());
        return View(response);
    }

    [Route("details/{serviceId}")]
    public async Task<IActionResult> Details(Guid serviceId)
    {
        var response = await _mediator.Send(new GetServiceByIdRequest { ServiceId = serviceId });
        return View(response);
    }
}
