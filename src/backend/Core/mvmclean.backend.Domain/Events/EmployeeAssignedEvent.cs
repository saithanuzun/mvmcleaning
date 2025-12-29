using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class EmployeeAssignedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid EmployeeId { get; }
    
    public EmployeeAssignedEvent(Guid bookingId, Guid employeeId): base()
    {
        BookingId = bookingId;
        EmployeeId = employeeId;
    }
}