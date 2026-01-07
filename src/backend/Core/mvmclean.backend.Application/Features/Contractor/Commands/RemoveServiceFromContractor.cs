using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class RemoveServiceFromContractorRequest : IRequest<RemoveServiceFromContractorResponse>
{
    public string ContractorId { get; set; }
    public string ServiceId { get; set; }
}

public class RemoveServiceFromContractorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class RemoveServiceFromContractorHandler : IRequestHandler<RemoveServiceFromContractorRequest, RemoveServiceFromContractorResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public RemoveServiceFromContractorHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<RemoveServiceFromContractorResponse> Handle(RemoveServiceFromContractorRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            return new RemoveServiceFromContractorResponse
            {
                Success = false,
                Message = "Invalid contractor ID"
            };

        if (!Guid.TryParse(request.ServiceId, out var serviceId))
            return new RemoveServiceFromContractorResponse
            {
                Success = false,
                Message = "Invalid service ID"
            };

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, noTracking: false);
        if (contractor == null)
            return new RemoveServiceFromContractorResponse
            {
                Success = false,
                Message = "Contractor not found"
            };

        contractor.RemoveService(serviceId);

        await _contractorRepository.SaveChangesAsync();

        return new RemoveServiceFromContractorResponse
        {
            Success = true,
            Message = "Service has been successfully removed from your profile"
        };
    }
}
