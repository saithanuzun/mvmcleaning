using mvmclean.backend.Domain.Common;
namespace mvmclean.backend.Domain.ValueObjects;

public class InvoiceLineItem : ValueObject
{
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);
    
    public InvoiceLineItem(string description, Money unitPrice, int quantity)
    {
        Description = description;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}