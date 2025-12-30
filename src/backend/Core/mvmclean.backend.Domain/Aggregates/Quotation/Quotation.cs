using mvmclean.backend.Domain.Aggregates.Quotation.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Quotation;

public class Quotation : AggregateRoot
{
    public PhoneNumber PhoneNumber { get; private set; }
    public Postcode Postcode { get; private set; }
    public Basket Basket { get; private set; }
    public Money Cost { get; private set; }

    private Quotation() { } // EF Core

    private Quotation(PhoneNumber phoneNumber, Postcode postcode, Basket basket)
    {
        PhoneNumber = phoneNumber;
        Postcode = postcode;
        Basket = basket;

        UpdateCost();
    }

    public static Quotation Create(string phoneNumber, string postcode)
    {
        return new Quotation(
            PhoneNumber.Create(phoneNumber),
            Postcode.Create(postcode),
            Basket.Create()
        );
    }

    public void UpdateCost()
    {
        Cost = Money.Zero();

        foreach (var item in Basket.BasketItems)
        {
            Cost += item.Price * item.Quantity;
        }
    }
}
