using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.Aggregates.Service.Entities;

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
    private readonly ICategoryRepository _categoryRepository;

    public CreateServiceHandler(IServiceRepository serviceRepository, ICategoryRepository categoryRepository)
    {
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
    }


    public async Task<CreateServiceResponse> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var duration = TimeSpan.FromMinutes(request.EstimatedDurationMinutes);
        var service = Service.Create(request.Name, request.Description, request.Shortcut, request.BasePrice, duration);

        var category =  _categoryRepository.Get(i => i.Name == request.Name).FirstOrDefault();
        
        if  (category == null)
        {
            Category newCategory;
            newCategory = Category.Create(request.Name);
            service.AddCategory(newCategory);
        }
        else
        {
            service.AddCategory(category);
        }

        
        await _serviceRepository.AddAsync(service);
        
        return new CreateServiceResponse
        {
            ServiceId = service.Id.ToString(),
        };
    }
}
