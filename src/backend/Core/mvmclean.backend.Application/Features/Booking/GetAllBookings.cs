using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;

namespace mvmclean.backend.Application.Features.Booking;

public class GetAllBookingsRequest : IRequest<GetAllBookingsResponse>
{
}

public class GetAllBookingsResponse
{
    public List<BookingDto> Bookings { get; set; } = new();
}

public class BookingDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid? ContractorId { get; set; }
    public string ContractorName { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; }
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

public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsRequest, GetAllBookingsResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<GetAllBookingsResponse> Handle(GetAllBookingsRequest request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetAll(false);

        var bookingDtos = bookings
            .Select(b => new BookingDto
            {
                Id = b.Id,
                PhoneNumber = b.PhoneNumber.Value,
                Postcode = b.Postcode.Value,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer?.FirstName + " " + b.Customer?.LastName,
                ContractorId = b.ContractorId,
                TotalPrice = b.TotalPrice.Amount,
                Currency = b.TotalPrice.Currency,
                ServiceItemCount = b.ServiceItems.Count(),
                Status = b.Status,
                CreationStatus = b.CreationStatus,
                ScheduledDate = b.ScheduledSlot?.StartTime,
                CreatedAt = b.CreatedAt
            })
            .ToList();

        return new GetAllBookingsResponse { Bookings = bookingDtos };
    }
}