using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Invoice.Entity;

public class InvoiceLineItem : Core.BaseClasses.Entity
{
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);
    
    public InvoiceLineItem(string description, decimal unitPrice, int quantity)
    {
        Description = description;
        UnitPrice = Money.Create(unitPrice);
        Quantity = quantity;
    }

    public InvoiceLineItem()
    {
        
    }

}