using MediatR;
using mvmclean.backend.Domain.Aggregates.SupportTicket;
using mvmclean.backend.Domain.Aggregates.SupportTicket.Enums;

namespace mvmclean.backend.Application.Features.SupportTicket.Queries;

public class GetAllSupportTicketsRequest : IRequest<GetAllSupportTicketsResponse>
{
}

public class GetAllSupportTicketsResponse
{
    public List<SupportTicketDto> Tickets { get; set; } = new();
}

public class SupportTicketDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? BookingId { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public int MessageCount { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string StatusBadgeClass => Status switch
    {
        TicketStatus.Open => "primary",
        TicketStatus.InProgress => "info",
        TicketStatus.Closed => "secondary",
        _ => "secondary"
    };
}

public class GetAllSupportTicketsHandler : IRequestHandler<GetAllSupportTicketsRequest, GetAllSupportTicketsResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public GetAllSupportTicketsHandler(ISupportTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<GetAllSupportTicketsResponse> Handle(GetAllSupportTicketsRequest request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetAll(false);

        var ticketDtos = tickets
            .Select(t => new SupportTicketDto
            {
                Id = t.Id,
                CustomerId = t.CustomerId,
                BookingId = t.BookingId,
                Subject = t.Subject,
                Description = t.Description,
                Status = t.Status,
                MessageCount = t.Messages.Count,
                AssignedToId = t.AssignedToId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return new GetAllSupportTicketsResponse { Tickets = ticketDtos };
    }
}
