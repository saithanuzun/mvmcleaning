using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

public class ServiceController : BaseApiController
{
    public ServiceController(IMediator mediator) : base(mediator)
    {
    }


    public IActionResult GetServicesByPostcode(string postcode)
    {
        return Ok();
    }
}