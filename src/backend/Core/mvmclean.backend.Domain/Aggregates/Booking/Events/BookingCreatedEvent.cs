using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class BookingCreatedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public PhoneNumber PhoneNumber { get; set; }
    public Postcode Postcode { get; set; }
    public BookingCreatedEvent(Guid bookingId, PhoneNumber phoneNumber, Postcode postcode) : base()
    {
        BookingId = bookingId;
        PhoneNumber = phoneNumber;
        Postcode = postcode;
    }
}