using MediatR;
using mvmclean.backend.Domain.Aggregates.Contact;

namespace mvmclean.backend.Application.Features.Contact.Queries;

public class GetAllContactsQuery : IRequest<GetAllContactsResponse>
{
}

public class GetAllContactsResponse
{
    public required List<ContactDto> Contacts { get; set; }
}

public class ContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MessageCount { get; set; }
}

public class GetAllContactsHandler : IRequestHandler<GetAllContactsQuery, GetAllContactsResponse>
{
    private readonly IContactRepository _contactRepository;

    public GetAllContactsHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<GetAllContactsResponse> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetAll();

        var contactDtos = contacts
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Subject = c.Subject,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                MessageCount = c.Messages.Count()
            })
            .ToList();

        return new GetAllContactsResponse { Contacts = contactDtos };
    }
}
