using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class ServiceController : BaseApiController
{
    public ServiceController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("bypostcode/{postcode}")]
    public IActionResult GetServicesByPostcode(string postcode)
    {
        // This is a stub - actual implementation is in ServicesController
        return Success("Use ServicesController instead");
    }
}