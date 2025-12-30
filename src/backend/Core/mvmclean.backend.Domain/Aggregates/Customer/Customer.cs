using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Customer;

public class Customer : Core.BaseClasses.AggregateRoot
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email? Email { get; private set; }
    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private Customer() { }

    public static Customer Create(PhoneNumber phoneNumber, string? email = null,
        string firstName = null, string lastName = null)
    {
        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = Email.Create(email)
        };
    }

    public void AddAddress(Address address)
    {
        if (!_addresses.Any(a => a.Equals(address)))
        {
            _addresses.Add(address);
        }
    }

    public string FullName => $"{FirstName} {LastName}";
}
