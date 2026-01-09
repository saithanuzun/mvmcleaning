using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class AvailabilityController : BaseApiController
{
    public AvailabilityController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get available time slots with contractor details
    /// </summary>
    [HttpGet("date")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] string postcode,
        [FromQuery] DateTime date,
        [FromQuery] int durationMinutes,
        [FromQuery] string contractorIds = "")
    {
        if (string.IsNullOrEmpty(postcode) && string.IsNullOrEmpty(contractorIds))
            return Error("Either postcode or contractor IDs are required");

        if (date < DateTime.Today)
            return Error("Date cannot be in the past");

        if (durationMinutes <= 0)
            return Error("Duration must be greater than 0");

        try
        {
            var request = new GetAvailableSlotsRequest
            {
                Postcode = postcode,
                Date = date,
                DurationMinutes = durationMinutes,
                ContractorIds = contractorIds
            };

            var response = await _mediator.Send(request);
            
            var message = response.AvailableSlots.Any()
                ? $"Found {response.AvailableSlots.Count} available slots"
                : "No available slots found for the selected date and duration";

            return Success(response, message);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving availability: {ex.Message}", 500);
        }
    }
}

