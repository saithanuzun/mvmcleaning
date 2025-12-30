using System.Collections.ObjectModel;
using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Services;


public interface IPricingService
{
    Money CalculatePrice(Money basePrice, Postcode postcode);
    Task<Money> CalculatePriceAsync(Money basePrice, Postcode postcode, CancellationToken cancellationToken = default);
    
}

public class PricingService: IPricingService
{
    private readonly ReadOnlyDictionary<string, PostcodePriceMultiplier> _postcodeMultipliers;

    public PricingService()
    {
        // Load postcode pricing rules from configuration/database
        _postcodeMultipliers = new ReadOnlyDictionary<string, PostcodePriceMultiplier>(
            new Dictionary<string, PostcodePriceMultiplier>
            {
                // Example: London postcodes
                ["SW1"] = new PostcodePriceMultiplier("SW1", 1.2m, Money.Create(10.00m)),
                ["W1"] = new PostcodePriceMultiplier("W1", 1.3m, Money.Create(15.00m)),
                ["NW1"] = new PostcodePriceMultiplier("NW1", 1.15m, Money.Create(5.00m)),
                
                // Example: Outside London
                ["M"] = new PostcodePriceMultiplier("M", 1.0m, Money.Create(20.00m)), // Manchester
                ["B"] = new PostcodePriceMultiplier("B", 1.0m, Money.Create(25.00m)), // Birmingham
                
                // Default
                ["DEFAULT"] = new PostcodePriceMultiplier("DEFAULT", 1.0m, Money.Create(0m))
            });
    }

    public Money CalculatePrice(Money basePrice, Postcode postcode)
    {
        var multiplier = GetPostcodeMultiplier(postcode);
        var adjustedPrice = basePrice.Multiply(multiplier.Multiplier);
        
        return adjustedPrice.Add(multiplier.AdditionalFee);
    }

    public async Task<Money> CalculatePriceAsync(Money basePrice, Postcode postcode, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(CalculatePrice(basePrice, postcode));
    }

    private PostcodePriceMultiplier GetPostcodeMultiplier(Postcode postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode.Value))
            return _postcodeMultipliers["DEFAULT"];

        var normalizedPostcode = postcode.Value.Normalize();
        
        foreach (var prefix in _postcodeMultipliers.Keys.Where(k => k != "DEFAULT"))
        {
            if (normalizedPostcode.StartsWith(prefix))
                return _postcodeMultipliers[prefix];
        }

        if (normalizedPostcode.Length > 0)
        {
            var areaCode = normalizedPostcode[0].ToString();
            if (_postcodeMultipliers.ContainsKey(areaCode))
                return _postcodeMultipliers[areaCode];
        }

        return _postcodeMultipliers["DEFAULT"];
    }
}
