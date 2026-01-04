using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.SharedKernel.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    private PhoneNumber() { }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty");

        var normalized = phoneNumber.Replace(" ", "").Replace("-", "");
            
        if (normalized.Length < 10 || normalized.Length > 15)
            throw new ArgumentException("Invalid phone number length");

        return new PhoneNumber { Value = normalized };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public override string ToString()
    {
        return Value;
    }
}