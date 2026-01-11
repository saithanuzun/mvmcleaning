using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;
using mvmclean.backend.Application.Services;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class CompleteBookingRequest : IRequest<CompleteBookingResponse>
{
    public string BookingId { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string CustomerPhone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Postcode { get; set; } = default!;
    public string ContractorId { get; set; } = default!;
    public ScheduledSlotDto ScheduledSlot { get; set; } = default!;
    public List<ServiceDto> Services { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "cash"; // 'cash' or 'card'
}

public class ScheduledSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class ServiceDto
{
    public string ServiceId { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CompleteBookingResponse
{
    public string BookingId { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string PaymentUrl { get; set; } = string.Empty;
    public string Message { get; set; } = default!;
}

public class CompleteBookingHandler : IRequestHandler<CompleteBookingRequest, CompleteBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IStripeService _stripeService;

    public CompleteBookingHandler(
        IBookingRepository bookingRepository,
        IContractorRepository contractorRepository,
        IStripeService stripeService)
    {
        _bookingRepository = bookingRepository;
        _contractorRepository = contractorRepository;
        _stripeService = stripeService;
    }

    public async Task<CompleteBookingResponse> Handle(CompleteBookingRequest request, CancellationToken cancellationToken)
    {
        // 1. Get booking by ID
        if (!Guid.TryParse(request.BookingId, out var bookingId))
            throw new ArgumentException("Invalid booking ID");

        var booking = await _bookingRepository.GetByIdAsync(bookingId, false);
        if (booking == null)
            throw new InvalidOperationException($"Booking {request.BookingId} not found");

        // Check if booking is already completed
        if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.InProgress)
            throw new InvalidOperationException($"Booking is already {booking.Status}. Cannot modify completed or in-progress bookings.");

        // 2. Get contractor
        if (!Guid.TryParse(request.ContractorId, out var contractorId))
            throw new ArgumentException("Invalid contractor ID");

        var contractor = await _contractorRepository.GetByIdAsync(contractorId, false);
        if (contractor == null)
            throw new InvalidOperationException($"Contractor {request.ContractorId} not found");

        // 3. Parse payment method
        PaymentType paymentType = request.PaymentMethod.ToLower() == "card" ? PaymentType.Card : PaymentType.Cash;
        var totalAmount = Money.Create(request.TotalAmount);

        // 4. Validate time slot
        var timeSlot = TimeSlot.Create(request.ScheduledSlot.StartTime, request.ScheduledSlot.EndTime);
        if (!contractor.IsAvailableAt(timeSlot))
            throw new InvalidOperationException("Contractor is not available for the selected time slot");

        // 5. Verify Stripe payment if card payment
        string? paymentLink = null;
        if (paymentType == PaymentType.Card)
        {
            try
            {
                // Get redirect URLs from configuration (set via appsettings or user-secrets)
                string successUrl = $"https://mvmcleaning.com/shop/payment-success";
                string cancelUrl = $"https://mvmcleaning.com/shop/payment-failed";

                paymentLink = await _stripeService.CreatePaymentLinkAsync(
                    bookingId,
                    booking.TotalPrice.Amount,
                    "gbp",
                    $"Booking payment for services",
                    successUrl,
                    cancelUrl
                );
                
                if (string.IsNullOrEmpty(paymentLink))
                    throw new InvalidOperationException("Failed to create Stripe payment link");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Stripe payment creation failed: {ex.Message}");
            }
        }

        // 6. Assign contractor to booking
        booking.SelectContractor(contractor);

        // 7. Assign customer details
        var email = Email.Create(request.CustomerEmail);
        var names = request.CustomerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = names.Length > 0 ? names[0] : "";
        var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";
        
        booking.AssignCustomer(
            email: email,
            FirstName: firstName,
            LastName: lastName,
            street: request.Address.Split(',')[0].Trim(),
            city: request.Address.Contains(',') ? request.Address.Split(',')[1].Trim() : "",
            additionalInfo: null
        );

        // 8. Assign service address
        var address = Address.Create(
            request.Address.Split(',')[0].Trim(),
            request.Address.Contains(',') ? request.Address.Split(',')[1].Trim() : "",
            Postcode.Create(request.Postcode),
            null
        );
        booking.AssignServiceAddress(address);

        // 9. Assign time slot (validate with contractor)
        booking.AssignTimeSlot(timeSlot, contractor);

        // 10. Create and assign payment based on payment method
        var payment = Payment.Create(bookingId, totalAmount, paymentType, paymentLink);
        
        // Mark as paid immediately if cash (will be collected on completion)
        if (paymentType == PaymentType.Cash)
        {
            payment.MarkAsCaptured();
        }
        
        booking.AssignPayment(payment);

        // 11. Confirm the booking
        booking.Confirm();

        // 12. Add unavailable slot to contractor (mark this time as booked)
        contractor.MarkAsUnavailable(timeSlot);

        // 13. Save changes
        await _bookingRepository.SaveChangesAsync();
        await _contractorRepository.SaveChangesAsync();

        return new CompleteBookingResponse
        {
            BookingId = booking.Id.ToString(),
            Status = booking.Status.ToString(),
            PaymentUrl = paymentLink ?? string.Empty,
            Message = paymentType == PaymentType.Cash 
                ? "Booking confirmed! Payment will be collected when the contractor arrives."
                : "Booking created. Please complete payment via Stripe."
        };
    }
}
