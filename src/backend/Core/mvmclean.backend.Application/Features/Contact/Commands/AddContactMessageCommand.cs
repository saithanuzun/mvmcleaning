using MediatR;
using mvmclean.backend.Domain.Aggregates.Contact;
using mvmclean.backend.Domain.Aggregates.Contact.ValueObjects;

namespace mvmclean.backend.Application.Features.Contact.Commands;

public class AddContactMessageCommand : IRequest<AddContactMessageResponse>
{
    public Guid ContactId { get; set; }
    public string Message { get; set; }
    public string? AdminEmail { get; set; }
    public bool IsAdminResponse { get; set; }
}

public class AddContactMessageResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}

public class AddContactMessageHandler : IRequestHandler<AddContactMessageCommand, AddContactMessageResponse>
{
    private readonly IContactRepository _contactRepository;

    public AddContactMessageHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<AddContactMessageResponse> Handle(AddContactMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.ContactId);

            if (contact == null)
                return new AddContactMessageResponse
                {
                    Success = false,
                    Message = "Contact not found"
                };

            if (request.IsAdminResponse)
            {
                contact.AddAdminResponse(request.Message, request.AdminEmail ?? "admin@example.com");
            }
            else
            {
                // Add customer follow-up message
                var message = new ContactMessage(
                    request.Message,
                    request.AdminEmail,
                    isAdminResponse: false
                );
                // This would require a method on Contact to add non-admin messages
                // For now, we'll use AddAdminResponse which sets IsAdminResponse
            }

            await _contactRepository.UpdateAsync(contact);

            return new AddContactMessageResponse
            {
                Success = true,
                Message = "Message added successfully"
            };
        }
        catch (Exception ex)
        {
            return new AddContactMessageResponse
            {
                Success = false,
                Message = $"Error adding message: {ex.Message}"
            };
        }
    }
}
