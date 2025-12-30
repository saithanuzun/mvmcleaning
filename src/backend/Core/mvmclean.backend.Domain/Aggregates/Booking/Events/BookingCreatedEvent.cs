using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

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