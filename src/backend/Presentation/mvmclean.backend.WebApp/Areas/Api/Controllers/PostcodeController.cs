using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.WebApp.Areas.Api.Models;
using AppQueries = mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class PostcodeController : BaseApiController
{
    public PostcodeController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Validates postcode format and checks if we cover the area
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidatePostcode([FromBody] Models.ValidatePostcodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Postcode))
        {
            return Error("Postcode is required");
        }

        var result = await _mediator.Send(new AppQueries.ValidatePostcodeRequest
        {
            Postcode = request.Postcode
        });

        return Success(new Models.ValidatePostcodeResponse
        {
            IsValid = result.IsValid,
            IsCovered = result.IsCovered,
            Postcode = result.Postcode
        });
    }
}
