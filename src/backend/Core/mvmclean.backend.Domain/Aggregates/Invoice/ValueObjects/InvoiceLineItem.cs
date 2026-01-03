using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Invoice.ValueObjects;

public class InvoiceLineItem : ValueObject
{
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);
    
    public static InvoiceLineItem  Create(string description, decimal unitPrice, int quantity)
    {
        return new InvoiceLineItem()
        {
            Description = description,
            UnitPrice = Money.Create(unitPrice),
            Quantity = quantity,
        };
    }
    
    private InvoiceLineItem() { }

    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}