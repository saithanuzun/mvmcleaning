using MediatR;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Application.Features.SupportTicket;

public class AssignTicketRequest : IRequest<AssignTicketResponse>
{
    public Guid TicketId { get; set; }
    public Guid ContractorId { get; set; }
}

public class AssignTicketResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Guid TicketId { get; set; }
}

public class AssignTicketHandler : IRequestHandler<AssignTicketRequest, AssignTicketResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public AssignTicketHandler(ISupportTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<AssignTicketResponse> Handle(AssignTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Support ticket with ID {request.TicketId} not found");

        ticket.AssignToContractor(request.ContractorId);
        
        await _ticketRepository.UpdateAsync(ticket);

        return new AssignTicketResponse
        {
            Success = true,
            Message = "Ticket assigned successfully",
            TicketId = ticket.Id
        };
    }
}
