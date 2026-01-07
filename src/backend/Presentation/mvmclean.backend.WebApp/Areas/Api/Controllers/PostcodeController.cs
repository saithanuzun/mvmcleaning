using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class PostcodeController : BaseApiController
{
    public PostcodeController(IMediator mediator) : base(mediator)
    {
    }

}
