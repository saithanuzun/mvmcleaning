using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class UpdateBookingStatusRequest : IRequest<UpdateBookingStatusResponse>
{
    public string BookingId { get; set; }
    public string Status { get; set; }
}

public class UpdateBookingStatusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class UpdateBookingStatusHandler : IRequestHandler<UpdateBookingStatusRequest, UpdateBookingStatusResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public UpdateBookingStatusHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<UpdateBookingStatusResponse> Handle(UpdateBookingStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.BookingId, out var bookingId))
            throw new ArgumentException("Invalid booking ID");

        var booking = await _bookingRepository.GetByIdAsync(bookingId, noTracking: false);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found");

        switch (request.Status?.ToLower())
        {
            case "confirmed":
                booking.Confirm();
                break;
            case "completed":
                booking.Complete();
                break;
            case "cancelled":
                booking.Cancel();
                break;
            default:
                return new UpdateBookingStatusResponse
                {
                    Success = false,
                    Message = $"Invalid status: {request.Status}"
                };
        }

        await _bookingRepository.SaveChangesAsync();

        return new UpdateBookingStatusResponse
        {
            Success = true,
            Message = $"Booking status updated to {request.Status}"
        };
    }
}
