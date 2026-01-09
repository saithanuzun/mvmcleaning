using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking.Queries;

public class GetBookingByPhoneAndPostcodeRequest : IRequest<GetBookingByPhoneAndPostcodeResponse>
{
    public string PhoneNumber { get; set; } = default!;
    public string Postcode { get; set; } = default!;
}

public class GetBookingByPhoneAndPostcodeResponse
{
    public string Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ServiceItemResponse> ServiceItems { get; set; } = new();
    public AddressResponse? ServiceAddress { get; set; }
    public ScheduledSlotResponse? ScheduledSlot { get; set; }
}

public class ServiceItemResponse
{
    public string ServiceName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class AddressResponse
{
    public string Street { get; set; }
    public string City { get; set; }
    public string? AdditionalInfo { get; set; }
}

public class ScheduledSlotResponse
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
}

public class GetBookingByPhoneAndPostcodeHandler : IRequestHandler<GetBookingByPhoneAndPostcodeRequest, GetBookingByPhoneAndPostcodeResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingByPhoneAndPostcodeHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<GetBookingByPhoneAndPostcodeResponse> Handle(GetBookingByPhoneAndPostcodeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new ArgumentException("Phone number is required");

        if (string.IsNullOrWhiteSpace(request.Postcode))
            throw new ArgumentException("Postcode is required");

        // Get all bookings and filter by phone and postcode
        var allBookings = await _bookingRepository.GetAll(false);
        
        var booking = allBookings.FirstOrDefault(b =>
            b.PhoneNumber.Value == request.PhoneNumber &&
            b.Postcode.Value.Equals(request.Postcode, StringComparison.OrdinalIgnoreCase));

        if (booking == null)
            throw new KeyNotFoundException($"No booking found for phone {request.PhoneNumber} and postcode {request.Postcode}");

        return new GetBookingByPhoneAndPostcodeResponse
        {
            Id = booking.Id.ToString(),
            PhoneNumber = booking.PhoneNumber.Value,
            Postcode = booking.Postcode.Value,
            CustomerName = booking.Customer?.FullName,
            CustomerEmail = booking.Customer?.Email,
            TotalPrice = booking.TotalPrice.Amount,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt,
            ServiceItems = booking.ServiceItems?.Select(item => new ServiceItemResponse
            {
                ServiceName = item.ServiceName,
                UnitPrice = item.UnitAdjustedPrice.Amount,
                Quantity = item.Quantity,
                TotalPrice = item.UnitAdjustedPrice.Amount * item.Quantity
            }).ToList() ?? new(),
            ServiceAddress = booking.ServiceAddress != null ? new AddressResponse
            {
                Street = booking.ServiceAddress.Street,
                City = booking.ServiceAddress.City,
                AdditionalInfo = booking.ServiceAddress.AdditionalInfo
            } : null,
            ScheduledSlot = booking.ScheduledSlot != null ? new ScheduledSlotResponse
            {
                StartTime = booking.ScheduledSlot.StartTime,
                EndTime = booking.ScheduledSlot.EndTime,
                DurationMinutes = (int)(booking.ScheduledSlot.EndTime - booking.ScheduledSlot.StartTime).TotalMinutes
            } : null
        };
    }
}
