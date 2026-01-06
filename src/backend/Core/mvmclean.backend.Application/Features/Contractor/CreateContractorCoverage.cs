using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class CreateContractorCoverageRequest : IRequest<CreateContractorCoverageResponse>
{
    public string ContractorId { get; set; }
    public string Postcode { get; set; }
}

public class CreateContractorCoverageResponse
{
    public Guid ContractorId { get; set; }
    public string Postcode { get; set; }
    public bool IsActive { get; set; }
    public string Message { get; set; }
}

public class CreateContractorCoverageHandler : IRequestHandler<CreateContractorCoverageRequest, CreateContractorCoverageResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public CreateContractorCoverageHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<CreateContractorCoverageResponse> Handle(CreateContractorCoverageRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            throw new ArgumentException("Invalid contractor ID");

        if (string.IsNullOrWhiteSpace(request.Postcode))
            throw new ArgumentException("Postcode is required");

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, noTracking: false);
        if (contractor == null)
            throw new KeyNotFoundException("Contractor not found");

        var postcode = Postcode.Create(request.Postcode);

        if (contractor.CoverageAreas.Any(c => c.Postcode.Value == postcode.Value))
        {
            return new CreateContractorCoverageResponse
            {
                ContractorId = contractorId,
                Postcode = request.Postcode,
                IsActive = true,
                Message = $"Coverage area '{request.Postcode}' already exists"
            };
        }
        
        contractor.AddCoverageArea(postcode);
        
        await _contractorRepository.SaveChangesAsync();
         
        return new CreateContractorCoverageResponse
        {
            ContractorId = contractorId,
            Postcode = request.Postcode,
            IsActive = true,
            Message = $"Coverage area '{request.Postcode}' has been successfully added for contractor"
        };
    }
}
