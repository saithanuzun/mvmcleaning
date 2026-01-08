using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class AssignContractorRequest : IRequest<AssignContractorResponse>
{
    public string BookingId  { get; set; }
    public string ContractorId { get; set; }
    public DateTime DateTime { get; set; }
    public TimeSpan Duration { get; set; }
}

public class AssignContractorResponse
{
    public string BookingId { get; set; }
    public string ContractorId { get; set; }
    public string Duration { get; set; }
}

public class AssignContractorHandler : IRequestHandler<AssignContractorRequest,AssignContractorResponse>
{
    private readonly IMediator _mediator;
    private readonly IContractorRepository _contractorRepository; 
    private readonly IBookingRepository  _bookingRepository;


    public AssignContractorHandler(IMediator mediator, IContractorRepository contractorRepository, IBookingRepository bookingRepository)
    {
        _mediator = mediator;
        _contractorRepository = contractorRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<AssignContractorResponse> Handle(AssignContractorRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(Guid.Parse(request.BookingId));
            
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId));
        
            
        booking.SelectContractor(contractor);
        
        // Convert unspecified DateTime to UTC for PostgreSQL compatibility
        var dateTime = request.DateTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DateTime, DateTimeKind.Utc)
            : request.DateTime.ToUniversalTime();
        
        booking.AssignTimeSlot(TimeSlot.Create(dateTime, dateTime + request.Duration), contractor);
        
        await _bookingRepository.SaveChangesAsync();

        return new AssignContractorResponse
        {
            BookingId = booking.Id.ToString(),
            ContractorId = contractor.Id.ToString(),
            Duration = request.Duration.ToString()
        };
    }
}