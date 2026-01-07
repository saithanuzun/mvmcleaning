using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class ServicesController : BaseApiController
{
    public ServicesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get all services available for a specific postcode with adjusted prices
    /// </summary>
    [HttpGet("bypostcode/{postcode}")]
    public async Task<IActionResult> GetServicesByPostcode(string postcode)
    {
        if (string.IsNullOrEmpty(postcode))
            return Error("Postcode is required");

        try
        {
            var request = new GetServicesByPostcodeRequest
            {
                Postcode = postcode
            };

            var response = await _mediator.Send(request);

            if (response == null || response.Count == 0)
                return Success(new List<object>(), $"No services available for postcode {postcode}");

            return Success(response, $"Found {response.Count} services available for postcode {postcode}");
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving services: {ex.Message}", 500);
        }
    }
}

