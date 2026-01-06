using MediatR;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Application.Features.SupportTicket;

public class AddMessageToTicketRequest : IRequest<AddMessageToTicketResponse>
{
    public Guid TicketId { get; set; }
    public Guid SenderId { get; set; }
    public string Message { get; set; }
    public bool IsInternalNote { get; set; } = false;
}

public class AddMessageToTicketResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Guid TicketId { get; set; }
}

public class AddMessageToTicketHandler : IRequestHandler<AddMessageToTicketRequest, AddMessageToTicketResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public AddMessageToTicketHandler(ISupportTicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<AddMessageToTicketResponse> Handle(AddMessageToTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

        if (ticket == null)
            throw new KeyNotFoundException($"Support ticket with ID {request.TicketId} not found");

        ticket.AddMessage(request.SenderId, request.Message, request.IsInternalNote);
        
        await _ticketRepository.UpdateAsync(ticket);

        return new AddMessageToTicketResponse
        {
            Success = true,
            Message = "Message added successfully",
            TicketId = ticket.Id
        };
    }
}
