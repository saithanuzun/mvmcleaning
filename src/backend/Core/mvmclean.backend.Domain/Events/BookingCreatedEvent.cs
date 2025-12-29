using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class BookingCreatedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid CustomerId { get; }

    public BookingCreatedEvent(Guid bookingId, Guid customerId ) : base()
    {
        BookingId = bookingId;
        CustomerId = customerId;
    }
}