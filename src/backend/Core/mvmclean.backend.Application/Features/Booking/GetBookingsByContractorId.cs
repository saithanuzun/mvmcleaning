using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking;

public class GetBookingsByContractorIdRequest : IRequest<List<GetBookingsByContractorIdResponse>>
{
    public string ContractorId { get; set; }
}

public class GetBookingsByContractorIdResponse
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

public class GetBookingsByContractorIdHandler : IRequestHandler<GetBookingsByContractorIdRequest,List<GetBookingsByContractorIdResponse>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingsByContractorIdHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<List<GetBookingsByContractorIdResponse>> Handle(GetBookingsByContractorIdRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            throw new ArgumentException("Invalid contractor id");

        var bookings =  _bookingRepository.Get(i=>i.ContractorId == contractorId).ToList();

        return bookings.Select(booking => new GetBookingsByContractorIdResponse
        {
            Id = booking.Id,
            PhoneNumber = booking.PhoneNumber.Value,
            Postcode = booking.Postcode.Value,

            ContractorId = booking.ContractorId,

            ServiceItems = booking.ServiceItems
                .Select(i => new BookingItem
                {
                    ServiceId = i.ServiceId,
                    UnitAdjustedPrice = i.UnitAdjustedPrice,
                    Quantity = i.Quantity
                })
                .ToList(),

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
        }).ToList();
    }
}