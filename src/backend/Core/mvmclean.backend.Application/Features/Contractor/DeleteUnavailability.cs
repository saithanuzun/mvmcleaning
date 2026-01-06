using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class DeleteUnavailabilityRequest : IRequest<DeleteUnavailabilityResponse>
{
    public string ContractorId { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class DeleteUnavailabilityResponse
{
    public string ContractorId { get; set; } = default!;
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class DeleteUnavailabilityHandler : IRequestHandler<DeleteUnavailabilityRequest, DeleteUnavailabilityResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public DeleteUnavailabilityHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<DeleteUnavailabilityResponse> Handle(DeleteUnavailabilityRequest request, CancellationToken cancellationToken)
    {
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId), noTracking: false);

        if (contractor == null)
        {
            throw new Exception($"Contractor not found: {request.ContractorId}");
        }

        // Convert to UTC if not already
        var startTimeUtc = request.StartTime.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc)
            : request.StartTime.ToUniversalTime();

        var endTimeUtc = request.EndTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc)
            : request.EndTime.ToUniversalTime();

        contractor.RemoveUnavailableSlot(TimeSlot.Create(startTimeUtc, endTimeUtc));

        await _contractorRepository.SaveChangesAsync();

        return new DeleteUnavailabilityResponse
        {
            ContractorId = contractor.Id.ToString(),
            Success = true,
            Message = "Unavailability slot removed successfully"
        };
    }
}
