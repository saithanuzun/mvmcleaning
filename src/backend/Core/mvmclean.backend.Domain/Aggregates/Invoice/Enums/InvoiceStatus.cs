namespace mvmclean.backend.Domain.Aggregates.Invoice.Enums;

public enum InvoiceStatus
{
    Draft =0,
    Sent = 1,
    Paid = 2 ,
    Overdue = 3,
    Cancelled = 4,
}