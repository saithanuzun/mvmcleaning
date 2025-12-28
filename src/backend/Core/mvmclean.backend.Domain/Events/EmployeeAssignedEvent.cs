using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class EmployeeAssignedEvent : IDomainEvent
{
    public Guid BookingId { get; }
    public Guid EmployeeId { get; }
    public DateTime OccurredOn { get; }
    
    public Guid EventId { get; }

    public EmployeeAssignedEvent(Guid bookingId, Guid employeeId)
    {
        EventId = Guid.NewGuid();
        BookingId = bookingId;
        EmployeeId = employeeId;
        OccurredOn = DateTime.UtcNow;
    }
}