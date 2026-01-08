using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class RemoveCartItemRequest : IRequest<RemoveCartItemResponse>
{
    public string BookingId { get; set; }
    public string ServiceItemId { get; set; }
}

public class RemoveCartItemResponse
{
    public string BookingId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalDurationMinutes { get; set; }
}

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemRequest, RemoveCartItemResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public RemoveCartItemHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<RemoveCartItemResponse> Handle(RemoveCartItemRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.BookingId))
            return new RemoveCartItemResponse
            {
                Success = false,
                Message = "Booking ID is required"
            };

        if (string.IsNullOrEmpty(request.ServiceItemId))
            return new RemoveCartItemResponse
            {
                Success = false,
                Message = "Service Item ID is required"
            };

        if (!Guid.TryParse(request.BookingId, out var bookingId))
            return new RemoveCartItemResponse
            {
                Success = false,
                Message = "Invalid Booking ID format"
            };

        if (!Guid.TryParse(request.ServiceItemId, out var serviceItemId))
            return new RemoveCartItemResponse
            {
                Success = false,
                Message = "Invalid Service Item ID format"
            };

        try
        {
            // Load booking with tracking enabled (noTracking: false) to modify it
            var booking = await _bookingRepository.GetByIdAsync(bookingId, noTracking: false);

            if (booking == null)
                return new RemoveCartItemResponse
                {
                    Success = false,
                    Message = "Booking not found"
                };

            // Use domain method to decrement service quantity from cart (removes if quantity 0)
            booking.RemoveServiceFromCart(serviceItemId);

            // Save changes
            await _bookingRepository.SaveChangesAsync();

            // Calculate total duration from remaining items
            // Note: Duration is multiplied by quantity for total service time
            int totalDurationMinutes = booking.ServiceItems
                .Sum(item => (int)item.Quantity * 60); // Default 60 min per service, adjust as needed

            return new RemoveCartItemResponse
            {
                BookingId = booking.Id.ToString(),
                Success = true,
                Message = "Service removed from cart successfully",
                TotalPrice = booking.TotalPrice.Amount,
                TotalDurationMinutes = totalDurationMinutes
            };
        }
        catch (Exception ex)
        {
            return new RemoveCartItemResponse
            {
                Success = false,
                Message = $"Error removing service from cart: {ex.Message}"
            };
        }
    }
}
