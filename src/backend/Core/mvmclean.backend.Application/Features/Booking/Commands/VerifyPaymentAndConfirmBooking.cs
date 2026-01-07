using MediatR;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class VerifyPaymentAndConfirmBookingRequest : IRequest<VerifyPaymentAndConfirmBookingResponse>
{
    public Guid BookingId { get; set; }
    public string SessionId { get; set; }
}

public class VerifyPaymentAndConfirmBookingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Guid BookingId { get; set; }
    public string Status { get; set; }
}

public class VerifyPaymentAndConfirmBookingHandler : IRequestHandler<VerifyPaymentAndConfirmBookingRequest, VerifyPaymentAndConfirmBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IStripeService _stripeService;
    private readonly IMailingService _mailingService;

    public VerifyPaymentAndConfirmBookingHandler(
        IBookingRepository bookingRepository,
        IStripeService stripeService,
        IMailingService mailingService)
    {
        _bookingRepository = bookingRepository;
        _stripeService = stripeService;
        _mailingService = mailingService;
    }

    public async Task<VerifyPaymentAndConfirmBookingResponse> Handle(
        VerifyPaymentAndConfirmBookingRequest request,
        CancellationToken cancellationToken)
    {
        // Verify payment with Stripe
        var isPaymentSuccessful = await _stripeService.VerifyPaymentAsync(request.SessionId);

        if (!isPaymentSuccessful)
        {
            return new VerifyPaymentAndConfirmBookingResponse
            {
                Success = false,
                Message = "Payment verification failed",
                BookingId = request.BookingId
            };
        }

        // Get booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            return new VerifyPaymentAndConfirmBookingResponse
            {
                Success = false,
                Message = "Booking not found",
                BookingId = request.BookingId
            };
        }

        // Confirm booking
        booking.Confirm();
        await _bookingRepository.UpdateAsync(booking);

        // Send confirmation email
        await _mailingService.SendBookingConfirmationAsync(
            booking.Customer?.Email?.Value ?? "",
            booking.Customer?.FullName ?? "Customer",
            booking.Id,
            booking.ScheduledSlot?.StartTime ?? DateTime.Now,
            booking.ServiceItems?.Select(s => s.ServiceName).ToList() ?? new List<string>(),
            booking.TotalPrice.Amount);

        return new VerifyPaymentAndConfirmBookingResponse
        {
            Success = true,
            Message = "Payment verified and booking confirmed",
            BookingId = booking.Id,
            Status = booking.Status.ToString()
        };
    }
}
