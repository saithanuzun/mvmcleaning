using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.WebApp.Areas.Api.Models;
using AppQueries = mvmclean.backend.Application.Features.Booking.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class AvailabilityController : BaseApiController
{
    public AvailabilityController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Gets available time slots for a specific date and postcode
    /// </summary>
    [HttpGet("date")]
    public async Task<IActionResult> GetAvailabilityByDate([FromQuery] string postcode, [FromQuery] DateTime date, [FromQuery] int durationMinutes)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return Error("Postcode is required");
        }

        if (date < DateTime.Today)
        {
            return Error("Cannot book for past dates");
        }

        try
        {
            var result = await _mediator.Send(new AppQueries.GetAvailableSlotsRequest
            {
                Postcode = postcode,
                Date = date,
                DurationMinutes = durationMinutes
            });

            return Success(new AvailabilityResponse
            {
                AvailableSlots = result.AvailableSlots.Select(s => new AvailabilitySlot
                {
                    ContractorId = s.ContractorId,
                    ContractorName = s.ContractorName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    DisplayTime = s.DisplayTime
                }).ToList(),
                Message = result.Message
            });
        }
        catch (Exception ex)
        {
            return Error($"Error fetching availability: {ex.Message}", 500);
        }
    }
}
