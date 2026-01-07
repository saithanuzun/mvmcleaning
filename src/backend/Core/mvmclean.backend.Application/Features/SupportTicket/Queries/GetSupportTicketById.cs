using MediatR;
using mvmclean.backend.Domain.Aggregates.SupportTicket;
using mvmclean.backend.Domain.Aggregates.SupportTicket.Enums;

namespace mvmclean.backend.Application.Features.SupportTicket.Queries;

public class GetSupportTicketByIdRequest : IRequest<GetSupportTicketByIdResponse>
{
    public Guid TicketId { get; set; }
}

public class GetSupportTicketByIdResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? BookingId { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public List<TicketMessageDto> Messages { get; set; } = new();
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TicketMessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }
    public bool IsInternalNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetSupportTicketByIdHandler : IRequestHandler<GetSupportTicketByIdRequest, GetSupportTicketByIdResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public GetSupportTicketByIdHandler(ISupportTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<GetSupportTicketByIdResponse> Handle(GetSupportTicketByIdRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Support ticket with ID {request.TicketId} not found");

        return new GetSupportTicketByIdResponse
        {
            Id = ticket.Id,
            CustomerId = ticket.CustomerId,
            BookingId = ticket.BookingId,
            Subject = ticket.Subject,
            Description = ticket.Description,
            Status = ticket.Status,
            Messages = ticket.Messages.Select(m => new TicketMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Message,
                IsInternalNote = m.IsInternalNote,
                CreatedAt = m.CreatedAt
            }).ToList(),
            AssignedToId = ticket.AssignedToId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
