using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class BookingConfirmedEvent : IDomainEvent
{
    public Guid BookingId { get; }
    public Guid EmployeeId { get; }
    public DateTime OccurredOn { get; }
    
    public Guid EventId { get; }

    public BookingConfirmedEvent(Guid bookingId, Guid employeeId)
    {
        EventId = Guid.NewGuid();
        BookingId = bookingId;
        EmployeeId = employeeId;
        OccurredOn = DateTime.UtcNow;
    }
}
