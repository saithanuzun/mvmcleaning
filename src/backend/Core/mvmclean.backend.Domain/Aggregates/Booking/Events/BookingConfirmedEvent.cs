using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class BookingConfirmedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid ContractorId { get; }
    
    public BookingConfirmedEvent(Guid bookingId, Guid contractorId): base()
    {
        BookingId = bookingId;
        ContractorId = contractorId;
    }
}
