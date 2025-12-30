using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.SharedKernel.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }
    public string NormalizedValue { get; }
    
    // Private constructor for EF Core
    private Email() { }
    
    private Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new Exceptions.DomainException("Email cannot be empty");
        
        if (!IsValidEmail(email))
            throw new Exceptions.DomainException($"Invalid email format: {email}");
        
        Value = email.Trim();
        NormalizedValue = NormalizeEmail(email);
    }
    
    public static Email Create(string email)
    {
        return new Email(email);
    }
    
    public static bool TryCreate(string email, out Email? result)
    {
        result = null;
        
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        if (!IsValidEmail(email))
            return false;
        
        result = new Email(email);
        return true;
    }
    
    private static bool IsValidEmail(string email)
    {
        if (email.Length > 256) // Common email length limit
            return false;
            
        try
        {
            // Use .NET's built-in email validation
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private static string NormalizeEmail(string email)
    {
        // Convert to lowercase and trim
        return email.Trim().ToLowerInvariant();
    }
    
    public string GetLocalPart()
    {
        var atIndex = NormalizedValue.IndexOf('@');
        return atIndex > 0 ? NormalizedValue.Substring(0, atIndex) : string.Empty;
    }
    
    public string GetDomain()
    {
        var atIndex = NormalizedValue.IndexOf('@');
        return atIndex > 0 ? NormalizedValue.Substring(atIndex + 1) : string.Empty;
    }
    
    public bool IsGmail()
    {
        return GetDomain() == "gmail.com";
    }
    
    public bool IsCorporateEmail()
    {
        var domain = GetDomain();
        var localPart = GetLocalPart();
        
        // Example corporate email checks
        return domain.EndsWith(".edu") || 
               domain.EndsWith(".gov") || 
               domain.EndsWith(".org") ||
               localPart.Contains("info") ||
               localPart.Contains("admin") ||
               localPart.Contains("support");
    }
    
    // Implicit conversion to string (optional)
    public static implicit operator string(Email email) => email.Value;
    
    // Explicit conversion from string (optional)
    public static explicit operator Email(string email) => Create(email);
    
    public override string ToString() => Value;
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Compare normalized values for equality
        yield return NormalizedValue;
    }
    
    // Operator overloads for convenience
    public static bool operator ==(Email left, Email right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(Email left, Email right)
    {
        return !Equals(left, right);
    }
    
    // Optional: Add hash code caching for performance
    private int? _cachedHashCode;
    
    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = NormalizedValue.GetHashCode();
        }
        return _cachedHashCode.Value;
    }
}
