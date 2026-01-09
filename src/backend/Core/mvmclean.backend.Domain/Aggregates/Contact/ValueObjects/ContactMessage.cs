using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contact.ValueObjects;

public class ContactMessage : ValueObject
{
    public Guid Id { get; private set; }
    public Guid ContactId { get; private set; }
    public string Message { get; private set; }
    public string? SenderEmail { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsAdminResponse { get; private set; }

    public ContactMessage(string message, string? senderEmail, bool isAdminResponse = false)
    {
        Id = Guid.NewGuid();
        Message = message;
        SenderEmail = senderEmail;
        CreatedAt = DateTime.UtcNow;
        IsAdminResponse = isAdminResponse;
    }

    private ContactMessage() { }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}
