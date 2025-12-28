using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.ValueObjects;

public class PostcodePriceMultiplier : ValueObject
{
    public string PostcodePrefix { get; private set; }
    public decimal Multiplier { get; private set; }
    public Money AdditionalFee { get; private set; }

    public PostcodePriceMultiplier(string postcodePrefix, decimal multiplier, Money additionalFee)
    {
        PostcodePrefix = postcodePrefix;
        Multiplier = multiplier;
        AdditionalFee = additionalFee;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PostcodePrefix;
        yield return Multiplier;
        yield return AdditionalFee;
    }
}
