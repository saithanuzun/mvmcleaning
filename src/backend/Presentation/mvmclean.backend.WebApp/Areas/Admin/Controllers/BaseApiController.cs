using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}