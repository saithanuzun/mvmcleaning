using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Queries;

public class GetServicePostcodePricingsRequest : IRequest<List<GetServicePostcodePricingsResponse>>
{
    public string ServiceId { get; set; }
}

public class GetServicePostcodePricingsResponse
{
    public string ServiceId { get; set; }
    public string PostcodeArea { get; set; }
    public decimal Multiplier { get; set; }
    public decimal FixedAdjustment { get; set; }
}

public class GetServicePostcodePricingsHandler : IRequestHandler<GetServicePostcodePricingsRequest, List<GetServicePostcodePricingsResponse>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicePostcodePricingsHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<List<GetServicePostcodePricingsResponse>> Handle(GetServicePostcodePricingsRequest request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(Guid.Parse(request.ServiceId));

        var pricings = service.PostcodePricings.Select(i=> new GetServicePostcodePricingsResponse
        {
            ServiceId = i.ServiceId.ToString(),
            PostcodeArea = i.Postcode.ToString(),
            Multiplier =  i.Multiplier,
            FixedAdjustment =  i.FixedAdjustment,
        }).ToList();
        
        
        
        return pricings;
    }
}