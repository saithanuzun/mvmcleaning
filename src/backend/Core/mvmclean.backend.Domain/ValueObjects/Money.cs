using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.ValueObjects;

public class Money : ValueObject, IComparable<Money>, IComparable
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private static readonly HashSet<string> ValidCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "GBP", "USD", "EUR", "JPY", "CAD", "AUD", "CHF", "CNY", "INR", "TRY"
    };

    private Money() { }

    public static Money Create(decimal amount, string currency = "GBP")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters", nameof(currency));

        if (!ValidCurrencies.Contains(currency.ToUpper()))
            throw new ArgumentException($"Unsupported currency: {currency}", nameof(currency));

        return new Money 
        { 
            Amount = amount,
            Currency = currency.ToUpperInvariant()
        };
    }

    public static Money Zero(string currency = "GBP") => Create(0m, currency);

    public Money Add(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} to {other.Currency}");

        return Create(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract {other.Currency} from {Currency}");

        return Create(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

        return Create(Amount * multiplier, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new ArgumentException("Divisor must be positive", nameof(divisor));

        return Create(Amount / divisor, Currency);
    }

    public bool IsZero() => Amount == 0m;
    public bool IsPositive() => Amount > 0m;
    public bool IsNegative() => Amount < 0m;
    public bool HasSameCurrency(Money other) => other != null && Currency == other.Currency;

    public static Money operator +(Money left, Money right)
    {
        if (left is null || right is null)
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));

        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left is null || right is null)
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));

        return left.Subtract(right);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        if (money is null)
            throw new ArgumentNullException(nameof(money));

        return money.Multiply(multiplier);
    }

    public static Money operator *(decimal multiplier, Money money) => money * multiplier;

    public static Money operator /(Money money, decimal divisor)
    {
        if (money is null)
            throw new ArgumentNullException(nameof(money));

        return money.Divide(divisor);
    }

    public static bool operator ==(Money left, Money right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Money left, Money right) => !(left == right);

    public static bool operator <(Money left, Money right)
    {
        if (left is null || right is null)
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));

        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare {left.Currency} with {right.Currency}");

        return left.Amount < right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        if (left is null || right is null)
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));

        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot compare {left.Currency} with {right.Currency}");

        return left.Amount > right.Amount;
    }

    public static bool operator <=(Money left, Money right) => !(left > right);
    public static bool operator >=(Money left, Money right) => !(left < right);

    // IComparable implementation
    public int CompareTo(Money other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot compare {Currency} with {other.Currency}");

        return Amount.CompareTo(other.Amount);
    }

    public int CompareTo(object obj)
    {
        if (obj is null) return 1;
        if (obj is not Money other)
            throw new ArgumentException($"Object must be of type {nameof(Money)}");

        return CompareTo(other);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:F2}";

    public string ToString(string format)
    {
        if (string.IsNullOrEmpty(format))
            return ToString();

        return format.ToUpperInvariant() switch
        {
            "C" => $"{Currency} {Amount:C2}",
            "F" => $"{Currency} {Amount:F2}",
            "N" => $"{Currency} {Amount:N2}",
            _ => throw new FormatException($"The '{format}' format string is not supported.")
        };
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return base.Equals(obj);
    }

    public override int GetHashCode() => base.GetHashCode();

    public Money Abs() => Create(Math.Abs(Amount), Currency);
    
    public Money Round(int decimals = 2, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        return Create(Math.Round(Amount, decimals, rounding), Currency);
    }

    public Money ConvertTo(string targetCurrency, decimal exchangeRate)
    {
        if (string.IsNullOrWhiteSpace(targetCurrency))
            throw new ArgumentException("Target currency cannot be null or empty", nameof(targetCurrency));

        if (exchangeRate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(exchangeRate));

        return Create(Amount * exchangeRate, targetCurrency.ToUpperInvariant());
    }
}