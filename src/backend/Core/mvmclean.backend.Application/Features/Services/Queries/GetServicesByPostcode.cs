using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Services.Queries;

public class GetServicesByPostcodeRequest : IRequest<List<GetServicesByPostcodeResponse>>
{
    public string Postcode { get; set; }
    public string PhoneNumber { get; set; }
}

public class GetServicesByPostcodeResponse
{
    public Guid ContractorId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public int Duration { get; set; }
}

public class GetServicesByPostcodeHandler : IRequestHandler<GetServicesByPostcodeRequest, List<GetServicesByPostcodeResponse>>
{
    private readonly IMediator _mediator;
    private readonly IContractorRepository  _contractorRepository;
    private readonly IServiceRepository _serviceRepository;
    
    
    public Task<List<GetServicesByPostcodeResponse>> Handle(GetServicesByPostcodeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}