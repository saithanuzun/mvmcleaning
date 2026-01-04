using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class ServiceAddedToCartEvent : DomainEvent
{
    public ServiceAddedToCartEvent(Guid serviceItemId, Money unitPrice, int quantity)
    {
        ServiceItemId = serviceItemId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
    

    public Guid ServiceItemId { get; set; }
    public Money UnitPrice { get; set; }
    public int Quantity { get; set; }
}