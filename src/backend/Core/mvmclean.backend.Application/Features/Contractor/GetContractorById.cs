using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;
using mvmclean.backend.Domain.Aggregates.Contractor.ValueObjects;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class GetContractorByIdRequest : IRequest<GetContractorByIdResponse>
{
    public string Id { get; set; }
}

public class GetContractorByIdResponse
{
    public string FullName { get; set; }

    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    
    public List<Review> Reviews { get; set; }
    
    public List<ServiceItem> Services { get; set; }

    public List<WorkingHours> WorkingHours { get; set; }

    public List<ContractorCoverage> CoverageAreas { get; set; }
    
    public List<TimeSlot> UnavailableSlots { get; set; }
}

public class GetContractorByIdHandler : IRequestHandler<GetContractorByIdRequest,GetContractorByIdResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public GetContractorByIdHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }


    public async Task<GetContractorByIdResponse> Handle(GetContractorByIdRequest request, CancellationToken cancellationToken)
    {
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.Id));

        return new GetContractorByIdResponse
        {
            FullName = contractor.FullName,
            Username = contractor.Username,
            Email = contractor.Email,
            PhoneNumber = contractor.PhoneNumber.ToString(),
            IsActive = contractor.IsActive,
            Reviews = contractor.Reviews.ToList(),
            Services = contractor.Services.ToList(),
            WorkingHours = contractor.WorkingHours.ToList(),
            CoverageAreas = contractor.CoverageAreas.ToList(),
            UnavailableSlots = contractor.UnavailableSlots.ToList(),
        };
    }
}