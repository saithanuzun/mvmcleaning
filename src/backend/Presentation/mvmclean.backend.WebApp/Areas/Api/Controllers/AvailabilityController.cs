using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class AvailabilityController : BaseApiController
{
    public AvailabilityController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Get available time slots for contractors on a specific date
    /// </summary>
    [HttpGet("date")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] string contractorIds,
        [FromQuery] DateTime date,
        [FromQuery] int durationMinutes)
    {
        if (string.IsNullOrEmpty(contractorIds))
            return Error("Contractor IDs are required");

        if (date < DateTime.Today)
            return Error("Date cannot be in the past");

        if (durationMinutes <= 0)
            return Error("Duration must be greater than 0");

        try
        {
            // Parse comma-separated contractor IDs
            var contractorIdList = contractorIds
                .Split(',')
                .Select(id => id.Trim())
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            if (!contractorIdList.Any())
                return Error("No valid contractor IDs provided");

            var request = new GetContractorAvailabilityByDayRequest
            {
                ContractorIds = contractorIdList,
                Date = date,
                Duration = TimeSpan.FromMinutes(durationMinutes)
            };

            var response = await _mediator.Send(request);

            if (response == null || response.Count == 0)
                return Success(new List<object>(), "No available slots found for the selected date and duration");

            // Group by contractor and format response
            var groupedByContractor = response
                .GroupBy(r => r.ContractorId)
                .Select(g => new
                {
                    contractorId = g.Key,
                    availableSlots = g
                        .Where(slot => slot.Available)
                        .Select(slot => new
                        {
                            startTime = slot.StartTime,
                            endTime = slot.EndTime
                        })
                        .ToList()
                })
                .ToList();

            return Success(groupedByContractor, $"Found {response.Count(r => r.Available)} available slots");
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving availability: {ex.Message}", 500);
        }
    }
}
