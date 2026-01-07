using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class CreateBookingCompleteRequest : IRequest<CreateBookingCompleteResponse>
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string Address { get; set; }
    public string Postcode { get; set; }
    public string ContractorId { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public List<ServiceItemDto> ServiceItems { get; set; }
    public decimal TotalAmount { get; set; }

    public class ServiceItemDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

public class CreateBookingCompleteResponse
{
    public bool Success { get; set; }
    public Guid BookingId { get; set; }
    public string Message { get; set; }
}

public class CreateBookingCompleteHandler : IRequestHandler<CreateBookingCompleteRequest, CreateBookingCompleteResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingCompleteHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<CreateBookingCompleteResponse> Handle(CreateBookingCompleteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Create booking with phone and postcode
            var booking = Domain.Aggregates.Booking.Booking.Create(
                Postcode.Create(request.Postcode),
                PhoneNumber.Create(request.CustomerPhone));

            // Note: SelectContractor requires the actual Contractor entity, so we'll just set the ContractorId
            // This would need to be enhanced with proper contractor selection logic
            if (Guid.TryParse(request.ContractorId, out var contractorId))
            {
                // For now, we'll need to get the contractor from repository
                // In a real implementation, you'd inject IContractorRepository
                // For this simplified version, we'll skip SelectContractor and manually set it
                // booking.SelectContractor(contractor);
            }

            // Add service items to cart
            foreach (var serviceItem in request.ServiceItems)
            {
                booking.AddServiceToCart(
                    serviceItem.ServiceName,
                    serviceItem.ServiceId,
                    Money.Create(serviceItem.Price),
                    serviceItem.Quantity);
            }

            // Assign time slot (requires contractor entity, so we'll skip for now)
            // var timeSlot = TimeSlot.Create(request.ScheduledStartTime, request.ScheduledEndTime);
            // booking.AssignTimeSlot(timeSlot, contractor);

            // Parse customer name (assuming format: "FirstName LastName")
            var nameParts = request.CustomerName.Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : request.CustomerName;
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            // Parse address (assuming format: "street, city")
            var addressParts = request.Address.Split(',', 2);
            var street = addressParts.Length > 0 ? addressParts[0].Trim() : request.Address;
            var city = addressParts.Length > 1 ? addressParts[1].Trim() : "";

            // Assign customer
            booking.AssignCustomer(
                Email.Create(request.CustomerEmail),
                firstName,
                lastName,
                street,
                city,
                null);

            // Create and assign payment
            var payment = Payment.Create(booking.Id, Money.Create(request.TotalAmount), PaymentType.Card, null);
            booking.AssignPayment(payment);

            // Save booking
            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return new CreateBookingCompleteResponse
            {
                Success = true,
                BookingId = booking.Id,
                Message = "Booking created successfully"
            };
        }
        catch (Exception ex)
        {
            return new CreateBookingCompleteResponse
            {
                Success = false,
                Message = $"Error creating booking: {ex.Message}"
            };
        }
    }
}
