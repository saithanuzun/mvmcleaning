using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class BookingConfirmedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid EmployeeId { get; }
    
    public BookingConfirmedEvent(Guid bookingId, Guid employeeId): base()
    {
        BookingId = bookingId;
        EmployeeId = employeeId;
    }
}
