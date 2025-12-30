using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Invoice.Events;

public class InvoiceOverdueEvent : DomainEvent
{
    public InvoiceOverdueEvent(Guid id, Guid customerId, decimal totalAmountAmount)
    {
        throw new NotImplementedException();
    }

    public DateTime OccurredOn { get; }
    public Guid EventId { get; }
}