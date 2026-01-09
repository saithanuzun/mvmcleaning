using MediatR;
using mvmclean.backend.Domain.Aggregates.Contact;

namespace mvmclean.backend.Application.Features.Contact.Queries;

public class GetContactByIdQuery : IRequest<GetContactByIdResponse>
{
    public Guid ContactId { get; set; }

    public GetContactByIdQuery(Guid contactId)
    {
        ContactId = contactId;
    }
}

public class GetContactByIdResponse
{
    public required ContactDetailsDto Contact { get; set; }
}

public class ContactDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public required List<ContactMessageDto> Messages { get; set; }
}

public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string? SenderEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAdminResponse { get; set; }
}

public class GetContactByIdHandler : IRequestHandler<GetContactByIdQuery, GetContactByIdResponse>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<GetContactByIdResponse> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId);

        if (contact == null)
            throw new KeyNotFoundException($"Contact with ID {request.ContactId} not found");

        var contactDetails = new ContactDetailsDto
        {
            Id = contact.Id,
            FullName = contact.FullName,
            Email = contact.Email,
            PhoneNumber = contact.PhoneNumber,
            Subject = contact.Subject,
            Message = contact.Message,
            Status = contact.Status.ToString(),
            CreatedAt = contact.CreatedAt,
            Messages = contact.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new ContactMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    SenderEmail = m.SenderEmail,
                    CreatedAt = m.CreatedAt,
                    IsAdminResponse = m.IsAdminResponse
                })
                .ToList()
        };

        return new GetContactByIdResponse { Contact = contactDetails };
    }
}
