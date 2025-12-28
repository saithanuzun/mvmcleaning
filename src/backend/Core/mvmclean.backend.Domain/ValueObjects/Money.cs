using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { }

    public static Money Create(decimal amount, string currency = "GBP")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");

        return new Money { Amount = amount, Currency = currency };
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return Create(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return Create(Amount * multiplier, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:F2}";

    public Money Subtract(Money basePrice)
    {
        return Create(this.Amount-basePrice.Amount, Currency);
    }
}
