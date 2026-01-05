using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Area("Api")]
[Route("Api")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}