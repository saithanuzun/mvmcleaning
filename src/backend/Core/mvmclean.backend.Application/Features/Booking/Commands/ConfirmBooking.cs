using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class ConfirmBookingRequest : IRequest<ConfirmBookingResponse>
{
    public Guid BookingId { get; set; }
}

public class ConfirmBookingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class ConfirmBookingHandler : IRequestHandler<ConfirmBookingRequest, ConfirmBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public ConfirmBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<ConfirmBookingResponse> Handle(ConfirmBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, noTracking: false);

            if (booking == null)
            {
                return new ConfirmBookingResponse
                {
                    Success = false,
                    Message = "Booking not found"
                };
            }

            booking.Confirm();
            await _bookingRepository.SaveChangesAsync();

            return new ConfirmBookingResponse
            {
                Success = true,
                Message = "Booking confirmed successfully"
            };
        }
        catch (Exception ex)
        {
            return new ConfirmBookingResponse
            {
                Success = false,
                Message = $"Error confirming booking: {ex.Message}"
            };
        }
    }
}
