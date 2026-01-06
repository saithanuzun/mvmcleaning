using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;
using mvmclean.backend.Domain.Aggregates.Contractor.ValueObjects;

namespace mvmclean.backend.Application.Features.Contractor;

public class SetWorkingDaysRequest : IRequest<SetWorkingDaysResponse>
{
    public string ContractorId { get; set; } = default!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsWorkingDay { get; set; } = true;
}

public class SetWorkingDaysResponse
{
    public string ContractorId { get; set; } = default!;
    public string DayOfWeek { get; set; } = default!;
    public bool Success { get; set; }
}

public class SetWorkingDaysHandler : IRequestHandler<SetWorkingDaysRequest, SetWorkingDaysResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public SetWorkingDaysHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<SetWorkingDaysResponse> Handle(SetWorkingDaysRequest request, CancellationToken cancellationToken)
    {
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId), noTracking: false);

        if (contractor == null)
        {
            throw new Exception($"Contractor not found: {request.ContractorId}");
        }

        WorkingHours workingHours;

        if (request.IsWorkingDay)
        {
            workingHours = WorkingHours.CreateWorkingDay(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime);
        }
        else
        {
            workingHours = WorkingHours.CreateDayOff(request.DayOfWeek);
        }

        contractor.AddWorkingHours(workingHours);

        await _contractorRepository.SaveChangesAsync();

        return new SetWorkingDaysResponse
        {
            ContractorId = contractor.Id.ToString(),
            DayOfWeek = request.DayOfWeek.ToString(),
            Success = true
        };
    }
}
