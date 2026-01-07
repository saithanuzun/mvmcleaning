using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class UpdateContractorStatusRequest : IRequest<UpdateContractorStatusResponse>
{
    public string ContractorId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateContractorStatusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class UpdateContractorStatusHandler : IRequestHandler<UpdateContractorStatusRequest, UpdateContractorStatusResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public UpdateContractorStatusHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<UpdateContractorStatusResponse> Handle(UpdateContractorStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            throw new ArgumentException("Invalid contractor ID");

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, noTracking: false);
        if (contractor == null)
            throw new KeyNotFoundException("Contractor not found");

        if (request.IsActive)
            contractor.Activate();
        else
            contractor.Deactivate();

        await _contractorRepository.SaveChangesAsync();

        return new UpdateContractorStatusResponse
        {
            Success = true,
            Message = $"Contractor {(request.IsActive ? "activated" : "deactivated")} successfully"
        };
    }
}
