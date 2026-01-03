using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class ContractorAssignedEvent(Guid id, Guid contractorId) : DomainEvent
{
    public Guid BookingId { get; set; }
    public Guid ContractorId { get; set; }
}