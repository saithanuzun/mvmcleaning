using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class Customer : Entity
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email? Email { get; private set; }
    public Address? Address { get; private set; }

    private readonly List<Message> _messages = new();
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

    private Customer()
    {
    }

    public static Customer Create(PhoneNumber phoneNumber, string? email = null,
        string? firstName = null, string? lastName = null)
    {
        return new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = Email.Create(email)
        };
    }

    public void AddMessage(Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        _messages.Add(message);
    }

    public string FullName => $"{FirstName} {LastName}";
}
