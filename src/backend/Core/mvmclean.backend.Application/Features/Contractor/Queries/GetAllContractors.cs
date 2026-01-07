using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Queries;

public class GetAllContractorsRequest : IRequest<GetAllContractorsResponse>
{
}

public class GetAllContractorsResponse
{
    public List<ContractorListDto> Contractors { get; set; } = new();
}

public class ContractorListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
    public int ServiceCount { get; set; }
    public int CoverageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusBadgeClass => IsActive ? "success" : "danger";
}

public class GetAllContractorsHandler : IRequestHandler<GetAllContractorsRequest, GetAllContractorsResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public GetAllContractorsHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<GetAllContractorsResponse> Handle(GetAllContractorsRequest request, CancellationToken cancellationToken)
    {
        var contractors = await _contractorRepository.GetAll(false);

        var contractorDtos = contractors
            .Select(c => new ContractorListDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Username = c.Username,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber.ToString(),
                IsActive = c.IsActive,
                ReviewCount = c.Reviews?.Count ?? 0,
                AverageRating = c.Reviews?.Any() == true ? c.Reviews.Average(r => r.Rating) : 0,
                ServiceCount = c.Services?.Count ?? 0,
                CoverageCount = c.CoverageAreas?.Count ?? 0,
                CreatedAt = c.CreatedAt
            })
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        return new GetAllContractorsResponse { Contractors = contractorDtos };
    }
}
