using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Quotation.Entities;

public class Basket : Entity
{
    public List<BasketItem> BasketItems { get; private set; } = new();

    private Basket(List<BasketItem> basketItems)
    {
        BasketItems = basketItems;
    }

    private Basket() { }

    public static Basket Create()
    {
        return new Basket(new List<BasketItem>());
    }

    public void AddItem(Service service, Money price, int quantity)
    {
        var existingItem = BasketItems
            .FirstOrDefault(x => x.ServiceId == service.Id);

        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        BasketItems.Add(new BasketItem(service, price, quantity));
    }

}
