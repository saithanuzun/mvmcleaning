using mvmclean.backend.Domain.Aggregates.Quotation.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Quotation;

public class Quotation : AggregateRoot
{
    public PhoneNumber PhoneNumber { get; private set; }
    public Postcode Postcode { get; private set; }
    
    private readonly List<BasketItem> _basketItems = new();
    public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();
    public Money Cost { get; private set; }

    private Quotation() { } 

    private Quotation(PhoneNumber phoneNumber, Postcode postcode)
    {
        PhoneNumber = phoneNumber;
        Postcode = postcode;
        Cost = Money.Zero();
    }

    public static Quotation Create(string phoneNumber, string postcode)
    {
        return new Quotation(
            PhoneNumber.Create(phoneNumber),
            Postcode.Create(postcode)
        );
    }

    public void AddBasketItem(BasketItem basketItem)
    {
        _basketItems.Add(basketItem); 
        UpdateCost();
    }
    
    public void RemoveBasketItem(BasketItem basketItem)
    {
        _basketItems.Remove(basketItem); 
        UpdateCost();
    }

    private void UpdateCost()
    {
        Cost = Money.Zero();

        foreach (var item in _basketItems) 
        {
            Cost = Cost.Add(item.Price.Multiply(item.Quantity));
        }
    }
}

