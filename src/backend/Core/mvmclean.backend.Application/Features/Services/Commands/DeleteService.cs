using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Commands;

public class DeleteServiceRequest : IRequest<DeleteServiceResponse>
{
    public Guid ServiceId { get; set; }
}

public class DeleteServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class DeleteServiceHandler : IRequestHandler<DeleteServiceRequest, DeleteServiceResponse>
{
    private readonly IServiceRepository _serviceRepository;

    public DeleteServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<DeleteServiceResponse> Handle(DeleteServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, noTracking: false);
        if (service == null)
            throw new KeyNotFoundException("Service not found");

        await _serviceRepository.DeleteAsync(service);

        return new DeleteServiceResponse
        {
            Success = true,
            Message = "Service deleted successfully"
        };
    }
}
