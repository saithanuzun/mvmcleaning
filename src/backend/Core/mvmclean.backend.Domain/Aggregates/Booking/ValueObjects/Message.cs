using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;

public class Message : ValueObject
{
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }

    private Message()
    {
    }

    public Message(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.", nameof(content));

        Content = content;
        SentAt = DateTime.UtcNow;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}