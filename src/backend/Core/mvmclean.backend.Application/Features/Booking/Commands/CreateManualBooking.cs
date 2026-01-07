using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class CreateManualBookingRequest : IRequest<CreateManualBookingResponse>
{
    // Contractor info
    public string ContractorId { get; set; } = default!;

    // Basic booking info
    public string PhoneNumber { get; set; } = default!;
    public string Postcode { get; set; } = default!;

    // Customer details
    public string CustomerFirstName { get; set; } = default!;
    public string CustomerLastName { get; set; } = default!;
    public string? CustomerEmail { get; set; }
    public string? CustomerStreet { get; set; }
    public string? CustomerCity { get; set; }
    public string? CustomerAdditionalInfo { get; set; }

    // Scheduling
    public DateTime ScheduledDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;

    // Services
    public List<ManualBookingServiceItem> ServiceItems { get; set; } = new();
}

public class ManualBookingServiceItem
{
    public string ServiceId { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CreateManualBookingResponse
{
    public string BookingId { get; set; } = default!;
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class CreateManualBookingHandler : IRequestHandler<CreateManualBookingRequest, CreateManualBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IContractorRepository _contractorRepository;

    public CreateManualBookingHandler(
        IBookingRepository bookingRepository,
        IContractorRepository contractorRepository)
    {
        _bookingRepository = bookingRepository;
        _contractorRepository = contractorRepository;
    }

    public async Task<CreateManualBookingResponse> Handle(CreateManualBookingRequest request, CancellationToken cancellationToken)
    {
        // Get the contractor
        var contractor = await _contractorRepository.GetByIdAsync(Guid.Parse(request.ContractorId));
        if (contractor == null)
        {
            throw new Exception($"Contractor not found: {request.ContractorId}");
        }

        // Create the booking
        var postcode = Postcode.Create(request.Postcode);
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        var booking = Domain.Aggregates.Booking.Booking.Create(postcode, phoneNumber);

        // Assign contractor
        booking.SelectContractor(contractor);

        // Assign time slot
        var duration = TimeSpan.FromMinutes(request.DurationMinutes);
        var timeSlot = TimeSlot.Create(request.ScheduledDateTime, request.ScheduledDateTime.Add(duration));
        booking.AssignTimeSlot(timeSlot, contractor);

        // Add services to cart
        foreach (var item in request.ServiceItems)
        {
            booking.AddServiceToCart(
                item.ServiceName,
                Guid.Parse(item.ServiceId),
                Money.Create(item.Price),
                item.Quantity);
        }

        // Assign customer
        var email = !string.IsNullOrWhiteSpace(request.CustomerEmail)
            ? Email.Create(request.CustomerEmail)
            : null;

        booking.AssignCustomer(
            email,
            request.CustomerFirstName,
            request.CustomerLastName,
            request.CustomerStreet,
            request.CustomerCity,
            request.CustomerAdditionalInfo);

        // Save the booking
        await _bookingRepository.AddAsync(booking);

        return new CreateManualBookingResponse
        {
            BookingId = booking.Id.ToString(),
            Success = true,
            Message = "Booking created successfully"
        };
    }
}
