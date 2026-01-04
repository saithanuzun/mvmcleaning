using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class GetContractorAvailabilityByDayRequest : IRequest<List<GetContractorAvailabilityByDayResponse>>
{
    public string ContractorId { get; set; } = default!;
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
}

public class GetContractorAvailabilityByDayResponse
{
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
    public bool Available { get; set; }
}


public class GetContractorAvailabilityByDayHandler : IRequestHandler<GetContractorAvailabilityByDayRequest,List<GetContractorAvailabilityByDayResponse>>
{
    private readonly IContractorRepository _contractorRepository;

    public GetContractorAvailabilityByDayHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<List<GetContractorAvailabilityByDayResponse>> Handle(GetContractorAvailabilityByDayRequest request, CancellationToken token)
    {
        var contractorId = Guid.Parse(request.ContractorId);

        var contractor = await _contractorRepository.GetByIdAsync(contractorId);
        if (contractor == null)
            throw new Exception("Contractor not found");

        var day = request.Date.Date;

        var workStart = day.AddHours(8).AddMinutes(30);   
        var workEnd   = day.AddHours(18).AddMinutes(30);  

        var step = TimeSpan.FromMinutes(30);
        var duration = request.Duration;

        var result = new List<GetContractorAvailabilityByDayResponse>();

        for (var start = workStart; start.Add(duration) <= workEnd; start = start.Add(step))
        {
            var slot = TimeSlot.Create(start, start.Add(duration));

            var available = contractor.IsAvailableAt(slot);

            result.Add(new GetContractorAvailabilityByDayResponse
            {
                StartTime = slot.StartTime.ToString("HH:mm"),
                EndTime = slot.EndTime.ToString("HH:mm"),
                Available = available
            });
        }

        return result;
    }
    
}
