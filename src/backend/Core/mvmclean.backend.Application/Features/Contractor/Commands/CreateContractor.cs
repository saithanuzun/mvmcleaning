using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class CreateContractorRequest : IRequest<CreateContractorResponse>
{
    public string FirstName { get; set; }
    public string  LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateContractorResponse
{
    public Guid ContractorId { get; set; }

}

public class CreateContractorHandler : IRequestHandler<CreateContractorRequest, CreateContractorResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public CreateContractorHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }


    public async Task<CreateContractorResponse> Handle(CreateContractorRequest request, CancellationToken cancellationToken)
    {
        
        var contractor = Domain.Aggregates.Contractor.Contractor.Create(firstName:request.FirstName, lastName:request.LastName, phoneNumber:request.PhoneNumber, email:request.Email, imageUrl:request.ImageUrl, username:request.Username, password:Encryptor.PasswordEncryptor.Encrypt(request.Password));
        
        var result = await _contractorRepository.AddAsync(contractor);
        
        return new CreateContractorResponse
        {
            ContractorId = contractor.Id,

        };
    }
}