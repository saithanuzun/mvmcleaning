using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class DeleteWorkingDayRequest : IRequest<DeleteWorkingDayResponse>
{
    public string ContractorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
}

public class DeleteWorkingDayResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class DeleteWorkingDayHandler : IRequestHandler<DeleteWorkingDayRequest, DeleteWorkingDayResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public DeleteWorkingDayHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<DeleteWorkingDayResponse> Handle(DeleteWorkingDayRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            return new DeleteWorkingDayResponse
            {
                Success = false,
                Message = "Invalid contractor ID"
            };

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, noTracking: false);
        if (contractor == null)
            return new DeleteWorkingDayResponse
            {
                Success = false,
                Message = "Contractor not found"
            };

        // Set working hours to not working day
        var workingHours = contractor.WorkingHours.FirstOrDefault(w => w.DayOfWeek == request.DayOfWeek);
        if (workingHours != null)
        {
            workingHours.SetAsNonWorkingDay();
        }

        await _contractorRepository.SaveChangesAsync();

        return new DeleteWorkingDayResponse
        {
            Success = true,
            Message = $"Working hours for {request.DayOfWeek} have been removed"
        };
    }
}
