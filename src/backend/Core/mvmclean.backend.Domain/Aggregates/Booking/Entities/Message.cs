using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class Message : Entity
{
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }

    public Customer Customer { get; set; }
    public Guid CustomerId { get; set; }
    
    public Message(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.", nameof(content));

        Content = content;
        SentAt = DateTime.UtcNow;
    }

}