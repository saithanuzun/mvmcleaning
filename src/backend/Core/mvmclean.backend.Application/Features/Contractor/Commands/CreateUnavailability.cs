using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class CreateUnavailabilityRequest : IRequest<CreateUnavailabilityResponse>
{
    public string ContractorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CreateUnavailabilityResponse
{
    public string ContractorId { get; set; }
}

public class CreateUnavailabilityHandler : IRequestHandler<CreateUnavailabilityRequest, CreateUnavailabilityResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public CreateUnavailabilityHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<CreateUnavailabilityResponse> Handle(CreateUnavailabilityRequest request, CancellationToken cancellationToken)
    {
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId), noTracking: false);

        // Convert to UTC if not already
        var startTimeUtc = request.StartTime.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc)
            : request.StartTime.ToUniversalTime();

        var endTimeUtc = request.EndTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc)
            : request.EndTime.ToUniversalTime();

        contractor.MarkAsUnavailable(TimeSlot.Create(startTimeUtc, endTimeUtc));

        await _contractorRepository.SaveChangesAsync();

        return new CreateUnavailabilityResponse
        {
            ContractorId = contractor.Id.ToString(),
        };
    }
}