using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Commands;

public class CreateServiceRequest : IRequest<CreateServiceResponse>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Shortcut { get; set; }
    public decimal BasePrice { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public string Category { get; set; }
}

public class CreateServiceResponse
{
    public string ServiceId { get; set; }
}

public class CreateServiceHandler : IRequestHandler<CreateServiceRequest,CreateServiceResponse>
{
    private readonly IServiceRepository _serviceRepository;

    public CreateServiceHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<CreateServiceResponse> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var duration = TimeSpan.FromMinutes(request.EstimatedDurationMinutes);
        
        // Create service with category as string
        var service = Service.Create(
            request.Name, 
            request.Description, 
            request.Shortcut, 
            request.BasePrice, 
            duration,
            request.Category
        );

        // Add and save service
        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();
        
        return new CreateServiceResponse
        {
            ServiceId = service.Id.ToString(),
        };
    }
}
