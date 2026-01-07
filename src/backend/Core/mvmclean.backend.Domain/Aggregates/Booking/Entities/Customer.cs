using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class Customer : Entity
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email? Email { get; private set; }
    public Address Address { get; set; }

    private readonly List<Message> _messages = [];
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

    public ICollection<Booking> Bookings { get; set; }

    private Customer()
    {
    }

    public static Customer Create(PhoneNumber phoneNumber, string? email = null,
        string? firstName = null, string? lastName = null, Address address = null)
    {
        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = string.IsNullOrWhiteSpace(email) ? null : Email.Create(email),
            Address = address,
        };
    }

    public void AddMessage(Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        _messages.Add(message);
    }

    public string FullName => $"{FirstName} {LastName}";
}
