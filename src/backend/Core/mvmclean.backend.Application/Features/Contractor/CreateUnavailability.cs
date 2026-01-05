using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class CreateUnavailabilityRequest : IRequest<CreateUnavailabilityResponse>
{
    public string ContractorId { get; set; }
    public DateTime Startime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CreateUnavailabilityResponse
{
    public string ContractorId { get; set; }
}

public class CreateUnavailabilityHandler : IRequestHandler<CreateUnavailabilityRequest,CreateUnavailabilityResponse>
{
    private readonly IContractorRepository  _contractorRepository;

    public CreateUnavailabilityHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<CreateUnavailabilityResponse> Handle(CreateUnavailabilityRequest request, CancellationToken cancellationToken)
    {
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId));
        
        contractor.MarkAsUnavailable(TimeSlot.Create(request.Startime, request.EndTime));
        
        await _contractorRepository.UpdateAsync(contractor);
        
        
        return new CreateUnavailabilityResponse
        {
            ContractorId = contractor.Id.ToString(),
        };
    }
}