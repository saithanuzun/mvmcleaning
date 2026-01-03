using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.SharedKernel.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }
    public string NormalizedValue { get; }
    
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
        if (email.Length > 256) 
            return false;
            
        try
        {
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
        
        return domain.EndsWith(".edu") || 
               domain.EndsWith(".gov") || 
               domain.EndsWith(".org") ||
               localPart.Contains("info") ||
               localPart.Contains("admin") ||
               localPart.Contains("support");
    }
    
    public static implicit operator string(Email email) => email.Value;
    
    public static explicit operator Email(string email) => Create(email);
    
    public override string ToString() => Value;
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return NormalizedValue;
    }
    
    public static bool operator ==(Email left, Email right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(Email left, Email right)
    {
        return !Equals(left, right);
    }
    
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
