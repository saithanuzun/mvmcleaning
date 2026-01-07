using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Queries;

public class GetServiceByIdRequest : IRequest<GetServiceByIdResponse>
{
    public Guid ServiceId { get; set; }
}

public class GetServiceByIdResponse
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Shortcut { get; set; }
    public string Duration { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }

    public decimal BasePrice { get; set; }
    
    public List<PostcodePricingDto> PostcodePricings { get; set; } = new();
}

public class PostcodePricingDto
{
    public string PostcodeArea { get; set; }
    public decimal Multiplier { get; set; }
    public decimal FixedAdjustment { get; set; }
}

public class GetServiceByIdHandler : IRequestHandler<GetServiceByIdRequest, GetServiceByIdResponse>
{
    private IServiceRepository  _serviceRepository;

    public GetServiceByIdHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<GetServiceByIdResponse> Handle(GetServiceByIdRequest request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);

        var response = new GetServiceByIdResponse
        {
            ServiceId = service.Id,
            Name = service.Name,
            Description = service.Description,
            Shortcut = service.Shortcut,
            Duration = service.EstimatedDuration.TotalMinutes.ToString(),
            CategoryId = service.CategoryId,
            CategoryName = service.Category?.Name ?? "Uncategorized",
            BasePrice = service.BasePrice.Amount,
            PostcodePricings = service.PostcodePricings.Select(pp => new PostcodePricingDto
            {
                PostcodeArea = pp.Postcode.Area,
                Multiplier = pp.Multiplier,
                FixedAdjustment = pp.FixedAdjustment
            }).ToList()
        };

        return response;
    }
}