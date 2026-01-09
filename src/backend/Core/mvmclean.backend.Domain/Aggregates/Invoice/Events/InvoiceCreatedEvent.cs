using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Invoice.Events;

public class InvoiceCreateEvent : DomainEvent
{
    public string InvoiceId { get; set; }
    public string Type { get; set; }
}