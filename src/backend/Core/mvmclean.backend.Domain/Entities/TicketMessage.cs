using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Entities;

public class TicketMessage : Entity
{
    public Guid SenderId { get; private set; }
    public string Message { get; private set; }
    public DateTime SentAt { get; private set; }
    public bool IsInternalNote { get; private set; }
    
    public TicketMessage(Guid senderId, string message, bool isInternalNote)
    {
        SenderId = senderId;
        Message = message;
        SentAt = DateTime.UtcNow;
        IsInternalNote = isInternalNote;
    }
}
