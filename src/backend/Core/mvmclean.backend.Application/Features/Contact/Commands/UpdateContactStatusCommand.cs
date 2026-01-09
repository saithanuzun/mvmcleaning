using MediatR;
using mvmclean.backend.Domain.Aggregates.Contact;

namespace mvmclean.backend.Application.Features.Contact.Commands;

public class UpdateContactStatusCommand : IRequest<UpdateContactStatusResponse>
{
    public Guid ContactId { get; set; }
    public string Status { get; set; }
}

public class UpdateContactStatusResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}

public class UpdateContactStatusHandler : IRequestHandler<UpdateContactStatusCommand, UpdateContactStatusResponse>
{
    private readonly IContactRepository _contactRepository;

    public UpdateContactStatusHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<UpdateContactStatusResponse> Handle(UpdateContactStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.ContactId);

            if (contact == null)
                return new UpdateContactStatusResponse
                {
                    Success = false,
                    Message = "Contact not found"
                };

            // Parse status enum
            if (!Enum.TryParse<Domain.Aggregates.Contact.Enums.ContactStatus>(request.Status, out var status))
                return new UpdateContactStatusResponse
                {
                    Success = false,
                    Message = "Invalid status"
                };

            switch (status)
            {
                case Domain.Aggregates.Contact.Enums.ContactStatus.Read:
                    contact.MarkAsRead();
                    break;
                case Domain.Aggregates.Contact.Enums.ContactStatus.Resolved:
                    contact.Resolve();
                    break;
                case Domain.Aggregates.Contact.Enums.ContactStatus.Closed:
                    contact.Close();
                    break;
            }

            await _contactRepository.UpdateAsync(contact);

            return new UpdateContactStatusResponse
            {
                Success = true,
                Message = $"Contact status updated to {status}"
            };
        }
        catch (Exception ex)
        {
            return new UpdateContactStatusResponse
            {
                Success = false,
                Message = $"Error updating contact status: {ex.Message}"
            };
        }
    }
}
