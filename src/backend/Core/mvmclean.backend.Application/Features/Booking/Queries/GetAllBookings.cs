using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;

namespace mvmclean.backend.Application.Features.Booking.Queries;

public class GetAllBookingsRequest : IRequest<List<GetAllBookingsResponse>>
{
}

public class GetAllBookingsResponse
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid? ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<ServiceItemDto> ServiceItems { get; set; } = new();
    public int ServiceItemCount { get; set; }
    public BookingStatus Status { get; set; }
    public BookingCreationStatus CreationStatus { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusBadgeClass => Status switch
    {
        BookingStatus.Pending => "warning",
        BookingStatus.Confirmed => "info",
        BookingStatus.InProgress => "primary",
        BookingStatus.Completed => "success",
        BookingStatus.Cancelled => "danger",
        _ => "secondary"
    };
}

public class ServiceItemDto
{
    public string ServiceName { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsRequest, List<GetAllBookingsResponse>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<GetAllBookingsResponse>> Handle(GetAllBookingsRequest request, CancellationToken cancellationToken)
    {
        var bookings = _bookingRepository.Get(
            predicate: null,
            noTracking: true,
            b => b.ServiceItems,
            b => b.Customer
        ).ToList();


        if (bookings == null || !bookings.Any())
            return new List<GetAllBookingsResponse>();

        var bookingResponses = bookings
            .Select(b => new GetAllBookingsResponse
            {
                Id = b.Id,
                PhoneNumber = b.PhoneNumber?.Value ?? string.Empty,
                Postcode = b.Postcode?.Value ?? string.Empty,
                CustomerId = b.CustomerId,
                CustomerName = string.IsNullOrEmpty(b.Customer?.FirstName) 
                    ? string.Empty 
                    : $"{b.Customer.FirstName} {b.Customer.LastName}".Trim(),
                ContractorId = b.ContractorId,
                ContractorName = string.Empty, // You might want to load Contractor entity too
                TotalPrice = b.TotalPrice?.Amount ?? 0,
                Currency = b.TotalPrice?.Currency ?? string.Empty,
                ServiceItems = b.ServiceItems?.Select(item => new ServiceItemDto
                {
                    ServiceName = item.ServiceName ?? string.Empty,
                    ServiceId = item.ServiceId,
                    UnitPrice = item.UnitAdjustedPrice?.Amount ?? 0,
                    Quantity = item.Quantity,
                    TotalPrice = (item.UnitAdjustedPrice?.Amount ?? 0) * item.Quantity
                }).ToList() ?? new List<ServiceItemDto>(),
                ServiceItemCount = b.ServiceItems?.Count ?? 0,
                Status = b.Status,
                CreationStatus = b.CreationStatus,
                ScheduledDate = b.ScheduledSlot?.StartTime,
                CreatedAt = b.CreatedAt
            })
            .ToList();

        return bookingResponses;
    }
}