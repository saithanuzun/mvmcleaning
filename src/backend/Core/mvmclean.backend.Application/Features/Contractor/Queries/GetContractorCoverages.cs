using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Queries;

public class GetContractorCoveragesRequest : IRequest<GetContractorCoveragesResponse>
{
    public string ContractorId { get; set; }
}

public class CoverageAreaDto
{
    public string PostcodeArea { get; set; }
    public bool IsActive { get; set; }
}

public class GetContractorCoveragesResponse
{
    public Guid ContractorId { get; set; }
    public List<CoverageAreaDto> CoverageAreas { get; set; }
    public int TotalCount { get; set; }
}

public class GetContractorCoveragesHandler : IRequestHandler<GetContractorCoveragesRequest, GetContractorCoveragesResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public GetContractorCoveragesHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<GetContractorCoveragesResponse> Handle(GetContractorCoveragesRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            throw new ArgumentException("Invalid contractor ID");

        var contractor = await _contractorRepository.GetByIdAsync(contractorId);
        if (contractor == null)
            throw new KeyNotFoundException("Contractor not found");

        var coverageAreas = contractor.CoverageAreas
            .Select(c => new CoverageAreaDto
            {
                PostcodeArea = c.Postcode.Area.Length > 3 
                    ? c.Postcode.Area.Substring(0, 3) 
                    : c.Postcode.Area,
                IsActive = c.IsActive
            })
            .ToList();

        return new GetContractorCoveragesResponse
        {
            ContractorId = contractorId,
            CoverageAreas = coverageAreas,
            TotalCount = coverageAreas.Count
        };
    }
}