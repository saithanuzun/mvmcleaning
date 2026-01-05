using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;
[Area("Admin")]
[Route("Admin")]
public class BaseAdminController : Controller
{
    protected readonly IMediator _mediator;
    
    public BaseAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }
}