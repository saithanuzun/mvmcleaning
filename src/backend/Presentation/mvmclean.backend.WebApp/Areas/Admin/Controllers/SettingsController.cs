using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/settings")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class SettingsController : BaseAdminController
{
    public SettingsController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }


    
}