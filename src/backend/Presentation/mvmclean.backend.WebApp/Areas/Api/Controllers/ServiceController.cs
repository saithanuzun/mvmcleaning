using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class ServiceController : BaseApiController
{
    public ServiceController(IMediator mediator) : base(mediator)
    {
    }


}