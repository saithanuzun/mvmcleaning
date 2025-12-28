using mvmclean.backend.Domain.ValueObjects;

namespace mvmclean.backend.Domain.AggregateRoot;

public class Customer : Common.AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private Customer() { }

    public static Customer Create(string firstName, string lastName, PhoneNumber phoneNumber, string? email = null)
    {
        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = email
        };
    }

    public void AddAddress(Address address)
    {
        if (!_addresses.Any(a => a.Equals(address)))
        {
            _addresses.Add(address);
        }
    }

    public void UpdateContactInfo(string firstName, string lastName, PhoneNumber phoneNumber, string? email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
}
