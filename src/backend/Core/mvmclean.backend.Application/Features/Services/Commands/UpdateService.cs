using System.ComponentModel.DataAnnotations;
using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Commands;

public class UpdateServiceRequest : IRequest<UpdateServiceResponse>
{
    public Guid ServiceId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Shortcut { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int EstimatedDurationMinutes { get; set; }
}

public class UpdateServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class UpdateServiceHandler : IRequestHandler<UpdateServiceRequest, UpdateServiceResponse>
{
    private readonly IServiceRepository _serviceRepository;

    public UpdateServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<UpdateServiceResponse> Handle(UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId, noTracking: false);
        if (service == null)
            throw new KeyNotFoundException("Service not found");

        var duration = TimeSpan.FromMinutes(request.EstimatedDurationMinutes);
        service.UpdateDetails(request.Name, request.Description, duration);
        service.UpdatePrice(request.BasePrice);
        service.UpdateShortcut(request.Shortcut);

        await _serviceRepository.SaveChangesAsync();

        return new UpdateServiceResponse
        {
            Success = true,
            Message = "Service updated successfully"
        };
    }
}
