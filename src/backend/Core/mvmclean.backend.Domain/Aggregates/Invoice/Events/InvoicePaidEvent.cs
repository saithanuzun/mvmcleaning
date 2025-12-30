using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Invoice.Events;

public class InvoicePaidEvent : DomainEvent
{
    public InvoicePaidEvent(Guid id, Guid bookingId, decimal totalAmountAmount)
    {
        throw new NotImplementedException();
    }

    public DateTime OccurredOn { get; }
    public Guid EventId { get; }
}
