using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class ContractorController : BaseApiController
{
    public ContractorController(IMediator mediator) : base(mediator)
    {
        
    }

    /// <summary>
    /// Get contractors available for a specific postcode
    /// </summary>
    [HttpGet("bypostcode/{postcode}")]
    public async Task<IActionResult> GetContractorsByPostcode(string postcode)
    {
        if (string.IsNullOrEmpty(postcode))
            return Error("Postcode is required");

        try
        {
            var request = new GetContractorsByPostcodeRequest
            {
                Postcode = postcode,
                BookingId = "" // Optional booking ID
            };

            var response = await _mediator.Send(request);
            return Success(response, $"Found {response.ContractorIds.Count} contractors for postcode {postcode}");
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving contractors: {ex.Message}", 500);
        }
    }

    [HttpGet("availability/day")]
    public async Task<IActionResult> GetContractorsAvailabilityByDay([FromQuery] GetContractorAvailabilityByDayRequest request)
    {
        try
        {
            var response = await _mediator.Send(request);
            return Success(response);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving availability: {ex.Message}", 500);
        }
    }
    
    
}