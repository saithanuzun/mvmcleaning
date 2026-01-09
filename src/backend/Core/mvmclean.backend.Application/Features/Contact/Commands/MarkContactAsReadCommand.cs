using MediatR;
using mvmclean.backend.Domain.Aggregates.Contact;

namespace mvmclean.backend.Application.Features.Contact.Commands;

public class MarkContactAsReadCommand : IRequest<MarkContactAsReadResponse>
{
    public Guid ContactId { get; set; }
}

public class MarkContactAsReadResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}

public class MarkContactAsReadHandler : IRequestHandler<MarkContactAsReadCommand, MarkContactAsReadResponse>
{
    private readonly IContactRepository _contactRepository;

    public MarkContactAsReadHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<MarkContactAsReadResponse> Handle(MarkContactAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.ContactId);

            if (contact == null)
                return new MarkContactAsReadResponse
                {
                    Success = false,
                    Message = "Contact not found"
                };

            contact.MarkAsRead();
            await _contactRepository.UpdateAsync(contact);

            return new MarkContactAsReadResponse
            {
                Success = true,
                Message = "Contact marked as read"
            };
        }
        catch (Exception ex)
        {
            return new MarkContactAsReadResponse
            {
                Success = false,
                Message = $"Error marking contact as read: {ex.Message}"
            };
        }
    }
}
