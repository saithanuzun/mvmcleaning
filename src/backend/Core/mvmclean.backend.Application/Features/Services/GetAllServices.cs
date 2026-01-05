using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services;

public class GetAllServicesRequest : IRequest<List<GetAllServicesResponse>>
{
    
}

public class GetAllServicesResponse
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Shortcut { get; set; }
    public string Duration { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }

    public decimal BasePrice { get; set; }
}

public class GetAllServicesHandler : IRequestHandler<GetAllServicesRequest, List<GetAllServicesResponse>>
{
    private IServiceRepository  _serviceRepository;

    public GetAllServicesHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<List<GetAllServicesResponse>> Handle(GetAllServicesRequest request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAll();

        return services.Select(service => new GetAllServicesResponse
            {
                ServiceId = service.Id,
                Name = service.Name,
                Description = service.Description,
                Shortcut = service.Shortcut,
                Duration = $"{(int)service.EstimatedDuration.TotalMinutes}",

                CategoryId = service.CategoryId,
                CategoryName = service.Category?.Name,

                BasePrice = service.BasePrice.Amount
            })
            .ToList();
    }
}