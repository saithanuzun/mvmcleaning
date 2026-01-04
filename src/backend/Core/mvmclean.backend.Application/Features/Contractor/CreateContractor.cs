using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor;

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
    private IContractorRepository _contractorRepository;

    public CreateContractorHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }


    public async Task<CreateContractorResponse> Handle(CreateContractorRequest request, CancellationToken cancellationToken)
    {
        
        var contractor = Domain.Aggregates.Contractor.Contractor.Create(request.FirstName, request.LastName, request.PhoneNumber, request.Email, request.ImageUrl, request.Username, request.Password);
        
        await _contractorRepository.AddAsync(contractor);
        
        return new CreateContractorResponse
        {
            ContractorId = contractor.Id,

        };
    }
}