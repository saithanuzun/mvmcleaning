using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SupportTicket.Entities;

public class TicketMessage : Entity
{
    public Guid SenderId { get; private set; }
    public string Message { get; private set; }
    public DateTime SentAt { get; private set; }
    public bool IsInternalNote { get; private set; }
    
    public SupportTicket SupportTicket { get; set; }
    public Guid SupportTicketId { get; set; }    
    public TicketMessage(Guid senderId, string message, bool isInternalNote) : base()
    {
        SenderId = senderId;
        Message = message;
        SentAt = DateTime.UtcNow;
        IsInternalNote = isInternalNote;
    }
    protected TicketMessage(): base()
    {
    }
    
    
    
}
