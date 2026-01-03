using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Quotation.Entities;

public class BasketItem : ValueObject
{
    public Guid ServiceId { get; private set; }
    public Money Price { get; private set; }
    public int Quantity { get; private set; }
    
    
    private BasketItem() { } // EF Core

    private BasketItem(Guid serviceId, Money price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        ServiceId = ServiceId;
        Price = price;
        Quantity = quantity;
    }

    public static BasketItem Create(Guid serviceId, Money price, int quantity)
    {
        return new BasketItem(serviceId, price, quantity);
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        Quantity += amount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
    
}
