using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Quotation.Entities;

public class BasketItem : Entity
{
    public Guid ServiceId { get; private set; }
    public Service Service { get; private set; }
    public Money Price { get; private set; }
    public int Quantity { get; private set; }

    private BasketItem() { } // EF Core

    public BasketItem(Service service, Money price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Service = service;
        ServiceId = service.Id;
        Price = price;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        Quantity += amount;
    }
}
