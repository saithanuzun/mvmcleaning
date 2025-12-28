using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class BookingCreatedEvent : IDomainEvent
{
    public Guid BookingId { get; }
    public Guid CustomerId { get; }
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    public BookingCreatedEvent(Guid bookingId, Guid customerId )
    {
        BookingId = bookingId;
        CustomerId = customerId;
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}