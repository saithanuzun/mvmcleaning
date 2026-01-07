using MediatR;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Services.Commands;

public class AddServicePostcodePricingRequest :  IRequest<AddServicePostcodePricingResponse>
{
    public string ServiceId { get; set; }
    public string Postcode { get; set; }
    public decimal FixedAdjustment { get; set; }
    public decimal Multiplier { get; set; }
}

public class AddServicePostcodePricingResponse
{
    public string ServiceId { get; set; }
    public string PostcodeArea { get; set; }   
}

public partial class AddServicePostcodePricingHandler : IRequestHandler<AddServicePostcodePricingRequest,AddServicePostcodePricingResponse>
{
    private readonly IServiceRepository _serviceRepository;

    public AddServicePostcodePricingHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }


    public async Task<AddServicePostcodePricingResponse> Handle(AddServicePostcodePricingRequest request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(Guid.Parse(request.ServiceId));
        
        service.AddPostcodePricing(Postcode.Create(request.Postcode), request.Multiplier, request.FixedAdjustment);

        return new AddServicePostcodePricingResponse
        {   
            ServiceId = service.Id.ToString(),
            PostcodeArea = Postcode.Create(request.Postcode).Area.ToString(),
        };
    }
}