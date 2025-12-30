using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.Aggregates.Invoice.Enums;
using mvmclean.backend.Domain.Aggregates.Invoice.Events;
using mvmclean.backend.Domain.Aggregates.Invoice.ValueObjects;
using mvmclean.backend.Domain.Core.Interfaces;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Invoice;

public class Invoice : Core.BaseClasses.AggregateRoot
{
    public string InvoiceNumber { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    
    public Money Subtotal { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    
    public InvoiceStatus Status { get; private set; }
    public PaymentTerms PaymentTerms { get; private set; }
    
    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private Invoice() { }
    
    public static Invoice CreateForBooking(Booking.Booking booking, PaymentTerms paymentTerms)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = GenerateInvoiceNumber(),
            BookingId = booking.Id,
            CustomerId = booking.CustomerId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(paymentTerms.DaysToPay),
            Subtotal = booking.TotalPrice,
            DiscountAmount = Money.Create(0),
            Status = InvoiceStatus.Draft,
            PaymentTerms = paymentTerms
        };
        
        invoice.CalculateTotal();
        return invoice;
    }
    
    public void AddLineItem(string description, Money unitPrice, int quantity)
    {
        var lineItem = new InvoiceLineItem(
            description, 
            unitPrice, 
            quantity
        );
        
        _lineItems.Add(lineItem);
        RecalculateTotals();
    }
    
    public void ApplyDiscount(Promotion.Promotion discount)
    {
        DiscountAmount = discount.ApplyDiscount(Subtotal);
        CalculateTotal();
    }
    
    
    public void MarkAsSent()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be sent");
        
        Status = InvoiceStatus.Sent;
        AddDomainEvent(new InvoiceSentEvent(Id, CustomerId, TotalAmount.Amount));
    }
    
    public void MarkAsPaid(DateTime paymentDate)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Invoice is already paid");
        
        Status = InvoiceStatus.Paid;
        PaidDate = paymentDate;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new InvoicePaidEvent(Id, BookingId, TotalAmount.Amount));
    }
    
    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Sent && DateTime.UtcNow > DueDate)
        {
            Status = InvoiceStatus.Overdue;
            AddDomainEvent(new InvoiceOverdueEvent(Id, CustomerId, TotalAmount.Amount));
        }
    }
    
    public void AddLateFee(Money lateFee)
    {
        if (Status != InvoiceStatus.Overdue)
            throw new InvalidOperationException("Late fees can only be added to overdue invoices");
        
        AddLineItem("Late Payment Fee", lateFee, 1);
        RecalculateTotals();
    }
    
    private void RecalculateTotals()
    {
        Subtotal = _lineItems.Aggregate(
            Money.Create(0), 
            (total, item) => total.Add(item.LineTotal)
        );
        
        CalculateTotal();
    }
    
    private void CalculateTotal()
    {
        TotalAmount = Subtotal
            .Subtract(DiscountAmount);
    }
    
    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}






