using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Queries;

public class GetBookingByIdRequest : IRequest<GetBookingByIdResponse>
{
    public string BookingId { get; set; }
}

public class GetBookingByIdResponse
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }

    public Guid? ContractorId { get; set; }

    public IReadOnlyList<BookingItem> ServiceItems { get; set; } = new List<BookingItem>();

    public decimal TotalPrice { get; set; }
    public string Currency { get; set; }

    public TimeSlot? ScheduledSlot { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public Address? ServiceAddress { get; set; }

    public Guid? PaymentId { get; set; }

    public BookingCreationStatus CreationStatus { get; set; }
    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdRequest, GetBookingByIdResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingByIdHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<GetBookingByIdResponse> Handle(GetBookingByIdRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.BookingId, out var bookingId))
            throw new ArgumentException("Invalid booking id");

        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new KeyNotFoundException("Booking not found");

        return new GetBookingByIdResponse
        {
            Id = booking.Id,
            PhoneNumber = booking.PhoneNumber.Value,
            Postcode = booking.Postcode.Value,

            ContractorId = booking.ContractorId,

            ServiceItems = booking.ServiceItems.ToList(),

            TotalPrice = booking.TotalPrice.Amount,
            Currency = booking.TotalPrice.Currency,

            ScheduledSlot = booking.ScheduledSlot,

            CustomerId = booking.CustomerId,
            Customer = booking.Customer,

            ServiceAddress = booking.ServiceAddress,

            PaymentId = booking.PaymentId,

            CreationStatus = booking.CreationStatus,
            Status = booking.Status,

            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}