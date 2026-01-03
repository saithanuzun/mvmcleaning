using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class CoverageController : BaseApiController
{
    public CoverageController(IMediator mediator) : base(mediator)
    {
    }


    public IActionResult CheckCoverage(string postcode)
    {

        return Ok();
    }
}