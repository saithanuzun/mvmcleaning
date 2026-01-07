using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Promotion.Enums;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Promotion;

public class Promotion : AggregateRoot
{
    public string Code { get; private set; }
    public string Description { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public Money MinimumOrderAmount { get; private set; }
    
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public int UsageLimit { get; private set; }
    public int UsedCount { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public List<Postcode> ApplicablePostcodes { get; private set; }
    
    private Promotion() { }
    
    public static Promotion Create(string code, string description, DiscountType discountType, 
        decimal discountValue, DateTime validFrom, DateTime validTo, int usageLimit = 0)
    {
        return new Promotion
        {
            Code = code,
            Description = description,
            DiscountType = discountType,
            DiscountValue = discountValue,
            ValidFrom = validFrom,
            ValidTo = validTo,
            UsageLimit = usageLimit,
            UsedCount = 0,
            IsActive = true,
            MinimumOrderAmount = Money.Create(0),
            ApplicablePostcodes = new List<Postcode>(),
        };
    }
    
    public Money ApplyDiscount(Money totalAmount)
    {
        if (!IsActive)
            throw new InvalidOperationException("Promotion is not active");
        
        if (DateTime.UtcNow < ValidFrom || DateTime.UtcNow > ValidTo)
            throw new InvalidOperationException("Promotion is not valid");
        
        if (UsageLimit > 0 && UsedCount >= UsageLimit)
            throw new InvalidOperationException("Promotion usage limit reached");
        
        if (totalAmount.Amount < MinimumOrderAmount.Amount)
            throw new InvalidOperationException($"Minimum order amount of {MinimumOrderAmount} required");
        
        Money discountAmount;
        switch (DiscountType)
        {
            case DiscountType.Percentage:
                discountAmount = totalAmount.Multiply(DiscountValue / 100m);
                break;
            case DiscountType.FixedAmount:
                discountAmount = Money.Create(DiscountValue);
                if (discountAmount.Amount > totalAmount.Amount)
                    discountAmount = totalAmount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        UsedCount++;
        return totalAmount.Subtract(discountAmount);
    }
    
    public void AddApplicablePostcode(Postcode postcodePrefix)
    {
        ApplicablePostcodes.Add(postcodePrefix);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void SetMinimumOrderAmount(decimal amount)
    {
        MinimumOrderAmount = Money.Create(amount);
    }
}
