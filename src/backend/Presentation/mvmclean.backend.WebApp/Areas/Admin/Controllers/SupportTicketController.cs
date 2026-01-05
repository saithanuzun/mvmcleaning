using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/SupportTicket")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class SupportTicketController : BaseAdminController
{
    public SupportTicketController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }


    
}