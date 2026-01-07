using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Admin;
using mvmclean.backend.Application.Features.Admin.Queries;
using mvmclean.backend.Application.Features.Admin.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin")]
[Authorize(AuthenticationSchemes = "AdminCookie")]
public class HomeController : BaseAdminController
{
    public HomeController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var stats = await _mediator.Send(new GetDashboardStatsRequest());
        return View(stats);
    }
}