using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.Aggregates.Service.Entities;

namespace mvmclean.backend.Application.Features.Services;

public class CreateServiceRequest : IRequest<CreateServiceResponse>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Shortcut { get; set; }
    public Decimal BasePrice { get; set; }
    public TimeSpan Duration { get; set; }
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
        
        var service = Service.Create(request.Name, request.Description, request.Shortcut, request.BasePrice, request.Duration);
        
        service.AddCategory(request.Category);
        
        await _serviceRepository.AddAsync(service);
        
        return new CreateServiceResponse
        {
            ServiceId = service.Id.ToString(),
        };
    }
}
