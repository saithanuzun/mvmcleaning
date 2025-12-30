using mvmclean.backend.Domain.Aggregates.SupportTicket.Entities;
using mvmclean.backend.Domain.Aggregates.SupportTicket.Enums;

namespace mvmclean.backend.Domain.Aggregates.SupportTicket;

public class SupportTicket : Core.BaseClasses.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid? BookingId { get; private set; }
    public string Subject { get; private set; }
    public string Description { get; private set; }
    public TicketStatus Status { get; private set; }
    
    private readonly List<TicketMessage> _messages = new();
    public IReadOnlyCollection<TicketMessage> Messages => _messages.AsReadOnly();
    
    public Guid? AssignedToId { get; private set; }
    public Contractor.Contractor AssignedTo { get; private set; }
    
    private SupportTicket() { }
    
    public static SupportTicket Create(Guid customerId, string subject, string description
        , Guid? bookingId = null)
    {
        return new SupportTicket
        {
            CustomerId = customerId,
            BookingId = bookingId,
            Subject = subject,
            Description = description,
            Status = TicketStatus.Open
        };
    }
    
    public void AddMessage(Guid senderId, string message, bool isInternalNote = false)
    {
        _messages.Add(new TicketMessage(senderId, message, isInternalNote));
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AssignToContractor(Guid contractorId)
    {
        AssignedToId = contractorId;
        Status = TicketStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Close(string resolution)
    {
        Status = TicketStatus.Closed;
        AddMessage(CustomerId, $"Ticket closed. Resolution: {resolution}");
        UpdatedAt = DateTime.UtcNow;
    }
}
