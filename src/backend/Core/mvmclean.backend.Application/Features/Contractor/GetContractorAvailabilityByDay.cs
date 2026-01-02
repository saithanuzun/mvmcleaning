using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor;

public class GetContractorAvailabilityByDayRequest : IRequest<List<GetContractorAvailabilityByDayResponse>>
{
    //Todo

    public DateTime DateTime { get; set; }

    public int Duration { get; set; }
}
public class GetContractorAvailabilityByDayResponse 
{
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool Available { get; set; }
}

public class GetContractorAvailabilityByDayHandler : IRequestHandler<GetContractorAvailabilityByDayRequest,List<GetContractorAvailabilityByDayResponse>>
{
    private readonly IContractorRepository _contractorRepository;
    
    
    public Task<List<GetContractorAvailabilityByDayResponse>> Handle(GetContractorAvailabilityByDayRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
