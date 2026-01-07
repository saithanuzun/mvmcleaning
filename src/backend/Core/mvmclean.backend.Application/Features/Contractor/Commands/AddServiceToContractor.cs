using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class AddServiceToContractorRequest : IRequest<AddServiceToContractorResponse>
{
    public string ContractorId { get; set; }
    public string ServiceId { get; set; }
}

public class AddServiceToContractorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class AddServiceToContractorHandler : IRequestHandler<AddServiceToContractorRequest, AddServiceToContractorResponse>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IServiceRepository _serviceRepository;

    public AddServiceToContractorHandler(
        IContractorRepository contractorRepository,
        IServiceRepository serviceRepository)
    {
        _contractorRepository = contractorRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<AddServiceToContractorResponse> Handle(AddServiceToContractorRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            return new AddServiceToContractorResponse
            {
                Success = false,
                Message = "Invalid contractor ID"
            };

        if (!Guid.TryParse(request.ServiceId, out var serviceId))
            return new AddServiceToContractorResponse
            {
                Success = false,
                Message = "Invalid service ID"
            };

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, noTracking: false);
        if (contractor == null)
            return new AddServiceToContractorResponse
            {
                Success = false,
                Message = "Contractor not found"
            };

        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null)
            return new AddServiceToContractorResponse
            {
                Success = false,
                Message = "Service not found"
            };

        var serviceItem = new ServiceItem
        {
            ContractorId = contractorId,
            ServiceId = serviceId,
            ServiceName = service.Name,
            Category = service.Category?.Name ?? "Uncategorized",
            Description = service.Description
        };

        contractor.AddService(serviceItem);

        await _contractorRepository.SaveChangesAsync();

        return new AddServiceToContractorResponse
        {
            Success = true,
            Message = $"Service '{service.Name}' has been successfully added to your profile"
        };
    }
}
