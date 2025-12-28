using mvmclean.backend.Domain.Common;
using mvmclean.backend.Domain.ValueObjects;

namespace mvmclean.backend.Domain.Entities;

public class PostcodePricing : Entity
{
    public Postcode Postcode { get; private set; }
    public Guid ServiceId { get; private set; }
    public Service Service { get; private set; }
    public Money Price { get; private set; }

    private PostcodePricing() { }

    public static PostcodePricing Create(Postcode postcode, Guid serviceId, Money price)
    {
        return new PostcodePricing
        {
            Postcode = postcode,
            ServiceId = serviceId,
            Price = price,
        };
    }

    public void UpdatePrice(Money newPrice)
    {
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

}
