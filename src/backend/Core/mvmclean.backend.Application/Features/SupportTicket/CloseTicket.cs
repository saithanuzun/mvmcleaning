using MediatR;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Application.Features.SupportTicket;

public class CloseTicketRequest : IRequest<CloseTicketResponse>
{
    public Guid TicketId { get; set; }
    public string Resolution { get; set; }
}

public class CloseTicketResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Guid TicketId { get; set; }
}

public class CloseTicketHandler : IRequestHandler<CloseTicketRequest, CloseTicketResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public CloseTicketHandler(ISupportTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<CloseTicketResponse> Handle(CloseTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Support ticket with ID {request.TicketId} not found");

        ticket.Close(request.Resolution);
        
        await _ticketRepository.UpdateAsync(ticket);

        return new CloseTicketResponse
        {
            Success = true,
            Message = "Ticket closed successfully",
            TicketId = ticket.Id
        };
    }
}
