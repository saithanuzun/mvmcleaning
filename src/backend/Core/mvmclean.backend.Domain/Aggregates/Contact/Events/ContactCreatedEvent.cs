using mvmclean.backend.Domain.Core;
using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contact.Events;

public class ContactCreatedEvent : DomainEvent
{
    public Guid ContactId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }

    public ContactCreatedEvent(
        Guid contactId,
        string fullName,
        string email,
        string? phoneNumber,
        string subject,
        string message)
    {
        ContactId = contactId;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Subject = subject;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }
}
