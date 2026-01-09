using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Invoice.Events;

public class InvoiceCreatedEvent : DomainEvent
{
    public InvoiceCreatedEvent(string invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public string InvoiceId { get; set; }
}