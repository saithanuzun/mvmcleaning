using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Queries;

public class GetAvailableSlotsRequest : IRequest<GetAvailableSlotsResponse>
{
    public string Postcode { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
}

public class GetAvailableSlotsResponse
{
    public List<AvailableSlotDto> AvailableSlots { get; set; } = new();
    public string Message { get; set; }
}

public class AvailableSlotDto
{
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DisplayTime { get; set; }
}

public class GetAvailableSlotsHandler : IRequestHandler<GetAvailableSlotsRequest, GetAvailableSlotsResponse>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetAvailableSlotsHandler(
        IContractorRepository contractorRepository,
        IBookingRepository bookingRepository)
    {
        _contractorRepository = contractorRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<GetAvailableSlotsResponse> Handle(GetAvailableSlotsRequest request, CancellationToken cancellationToken)
    {
        if (request.Date < DateTime.Today)
        {
            return new GetAvailableSlotsResponse
            {
                AvailableSlots = new List<AvailableSlotDto>(),
                Message = "Cannot book for past dates"
            };
        }

        // Get contractors covering this postcode
        var allContractors = await _contractorRepository.GetAll(false);
        var availableContractors = allContractors
            .Where(c => c.IsActive &&
                       c.CoverageAreas != null &&
                       c.CoverageAreas.Any(ca => ca.Postcode.Value.Equals(request.Postcode, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (!availableContractors.Any())
        {
            return new GetAvailableSlotsResponse
            {
                AvailableSlots = new List<AvailableSlotDto>(),
                Message = "No contractors available for this area"
            };
        }

        var availableSlots = new List<AvailableSlotDto>();

        foreach (var contractor in availableContractors)
        {
            // Check if contractor works on this day
            var dayOfWeek = request.Date.DayOfWeek;
            var workingHours = contractor.WorkingHours?.FirstOrDefault(w => w.DayOfWeek == dayOfWeek && w.IsWorkingDay);

            if (workingHours == null) continue;

            // Get contractor's bookings for this date
            var allBookings = _bookingRepository.Get(b => b.ContractorId == contractor.Id).ToList();
            var dayBookings = allBookings
                .Where(b => b.ScheduledSlot != null &&
                           b.ScheduledSlot.StartTime.Date == request.Date.Date &&
                           b.Status != BookingStatus.Cancelled)
                .ToList();

            // Get unavailable slots
            var unavailableSlots = contractor.UnavailableSlots?
                .Where(u => u.StartTime.Date == request.Date.Date)
                .ToList() ?? new List<TimeSlot>();

            // Generate time slots
            var startTime = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day,
                workingHours.StartTime.Hour, workingHours.StartTime.Minute, 0);
            var endTime = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day,
                workingHours.EndTime.Hour, workingHours.EndTime.Minute, 0);

            var currentSlot = startTime;
            while (currentSlot.AddMinutes(request.DurationMinutes) <= endTime)
            {
                var slotEnd = currentSlot.AddMinutes(request.DurationMinutes);

                // Check if slot conflicts with existing bookings
                var hasConflict = dayBookings.Any(b =>
                    (currentSlot >= b.ScheduledSlot.StartTime && currentSlot < b.ScheduledSlot.EndTime) ||
                    (slotEnd > b.ScheduledSlot.StartTime && slotEnd <= b.ScheduledSlot.EndTime) ||
                    (currentSlot <= b.ScheduledSlot.StartTime && slotEnd >= b.ScheduledSlot.EndTime));

                // Check if slot conflicts with unavailable periods
                var hasUnavailability = unavailableSlots.Any(u =>
                    (currentSlot >= u.StartTime && currentSlot < u.EndTime) ||
                    (slotEnd > u.StartTime && slotEnd <= u.EndTime) ||
                    (currentSlot <= u.StartTime && slotEnd >= u.EndTime));

                if (!hasConflict && !hasUnavailability)
                {
                    availableSlots.Add(new AvailableSlotDto
                    {
                        ContractorId = contractor.Id,
                        ContractorName = contractor.FullName,
                        StartTime = currentSlot,
                        EndTime = slotEnd,
                        DisplayTime = currentSlot.ToString("HH:mm") + " - " + slotEnd.ToString("HH:mm")
                    });
                }

                currentSlot = currentSlot.AddMinutes(30); // 30-minute intervals
            }
        }

        return new GetAvailableSlotsResponse
        {
            AvailableSlots = availableSlots.OrderBy(s => s.StartTime).ToList()
        };
    }
}
