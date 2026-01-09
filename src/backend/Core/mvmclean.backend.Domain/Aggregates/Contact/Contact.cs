using mvmclean.backend.Domain.Aggregates.Contact.Enums;
using mvmclean.backend.Domain.Aggregates.Contact.Events;
using mvmclean.backend.Domain.Aggregates.Contact.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Contact;

public class Contact : Core.BaseClasses.AggregateRoot
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public ContactStatus Status { get; private set; }
    
    private readonly List<ContactMessage> _messages = new();
    public IReadOnlyCollection<ContactMessage> Messages => _messages.AsReadOnly();

    private Contact() { }

    public static Contact Create(
        string fullName,
        string email,
        string? phoneNumber,
        string subject,
        string message)
    {
        var contact = new Contact
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            Subject = subject,
            Message = message,
            Status = ContactStatus.New
        };

        // Add initial message
        contact._messages.Add(new ContactMessage(message, email, isAdminResponse: false));

        // Raise domain event
        contact.AddDomainEvent(new ContactCreatedEvent(
            contactId: contact.Id,
            fullName: fullName,
            email: email,
            phoneNumber: phoneNumber,
            subject: subject,
            message: message
        ));

        return contact;
    }

    public void MarkAsRead()
    {
        Status = ContactStatus.Read;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAdminResponse(string responseMessage, string adminEmail)
    {
        _messages.Add(new ContactMessage(responseMessage, adminEmail, isAdminResponse: true));
        Status = ContactStatus.Responded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        Status = ContactStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        Status = ContactStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}
