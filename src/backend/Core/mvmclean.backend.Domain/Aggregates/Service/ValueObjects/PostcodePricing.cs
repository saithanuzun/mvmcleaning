using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Service.ValueObjects;

public class PostcodePricing : ValueObject
{
    public Guid ServiceId { get; private set; }
    public Postcode Postcode { get; private set; }
    public decimal Multiplier { get; private set; } 
    public decimal FixedAdjustment { get; private set; }

   
    private PostcodePricing() { }

    public static PostcodePricing Create(Guid serviceId, Postcode postcode, decimal multiplier, decimal fixedAdjustment)
    {
        if (serviceId == Guid.Empty)
            throw new Exception("Service ID is required");
        
        if (postcode == null)
            throw new Exception("Postcode is required");
        
        if (multiplier < 0)
            throw new Exception("Multiplier cannot be negative");
        
        if (multiplier > 3.0m)
            throw new Exception("Multiplier cannot exceed 300%");

        return new PostcodePricing
        {
            ServiceId = serviceId,
            Postcode = postcode,
            Multiplier = multiplier,
            FixedAdjustment = fixedAdjustment,
        };
    }
    public Money CalculateAdjustedPrice(Money basePrice)
    {
        var adjustedAmount = (basePrice.Amount * Multiplier) + FixedAdjustment;
        
        // Ensure minimum price if needed
        if (adjustedAmount < 0)
            adjustedAmount = 0;
            
        return Money.Create(adjustedAmount);
    }
    

    public void UpdatePricing(decimal newMultiplier, decimal newFixedAdjustment)
    {
        if (newMultiplier < 0)
            throw new Exception("Multiplier cannot be negative");
        
        if (newMultiplier > 3.0m)
            throw new Exception("Multiplier cannot exceed 300%");

        Multiplier = newMultiplier;
        FixedAdjustment = newFixedAdjustment;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}