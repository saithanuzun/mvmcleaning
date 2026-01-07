using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor.Queries;

public class GetContractorAvailabilityByDayRequest : IRequest<List<GetContractorAvailabilityByDayResponse>>
{
    public List<string> ContractorIds { get; set; } = default!;
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
}

public class GetContractorAvailabilityByDayResponse
{
    public string ContractorId { get; set; }
    public string StartTime { get; set; } = default!;
    public string EndTime { get; set; } = default!;
    public bool Available { get; set; }
}

public class GetContractorAvailabilityByDayHandler : IRequestHandler<GetContractorAvailabilityByDayRequest,
    List<GetContractorAvailabilityByDayResponse>>
{
    private readonly IContractorRepository _contractorRepository;

    public GetContractorAvailabilityByDayHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<List<GetContractorAvailabilityByDayResponse>> Handle(
        GetContractorAvailabilityByDayRequest request,
        CancellationToken token)
    {
        if (!request.ContractorIds.Any())
            throw new Exception("No contractors provided");

        var contractorIds = request.ContractorIds.Select(Guid.Parse).ToList();

        var contractors = new List<Domain.Aggregates.Contractor.Contractor>();

        foreach (var id in contractorIds)
        {
            var contractor = await _contractorRepository.GetByIdAsync(id);

            if (contractor == null)
                throw new Exception($"Contractor not found: {id}");

            contractors.Add(contractor);
        }

        if (contractors.Count != contractorIds.Count)
            throw new Exception("One or more contractors not found");

        var day = request.Date.Date;

        var workStart = day.AddHours(8).AddMinutes(30);
        var workEnd = day.AddHours(18).AddMinutes(30);

        var step = TimeSpan.FromMinutes(30);
        var duration = request.Duration;

        var result = new List<GetContractorAvailabilityByDayResponse>();

        for (var start = workStart; start.Add(duration) <= workEnd; start = start.Add(step))
        {
            var slot = TimeSlot.Create(start, start.Add(duration));

            foreach (var contractor in contractors)
            {
                var available = contractor.IsAvailableAt(slot);

                result.Add(new GetContractorAvailabilityByDayResponse
                {
                    ContractorId = contractor.Id.ToString(),
                    StartTime = slot.StartTime.ToString("HH:mm"),
                    EndTime = slot.EndTime.ToString("HH:mm"),
                    Available = available
                });
            }
        }

        return result;
    }
}