using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

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
