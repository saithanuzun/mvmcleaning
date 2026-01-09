using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Promotion;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class ApplyPromotionToBookingRequest : IRequest<ApplyPromotionToBookingResponse>
{
    public string BookingId { get; set; } = default!;
    public string PromotionCode { get; set; } = default!;
}

public class ApplyPromotionToBookingResponse
{
    public string BookingId { get; set; }
    public string PromotionCode { get; set; }
    public decimal OriginalTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NewTotal { get; set; }
    public string Message { get; set; }
}

public class ApplyPromotionToBookingHandler : IRequestHandler<ApplyPromotionToBookingRequest, ApplyPromotionToBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPromotionRepository _promotionRepository;

    public ApplyPromotionToBookingHandler(
        IBookingRepository bookingRepository,
        IPromotionRepository promotionRepository)
    {
        _bookingRepository = bookingRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<ApplyPromotionToBookingResponse> Handle(
        ApplyPromotionToBookingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.BookingId))
            throw new ArgumentException("Booking ID is required");

        if (string.IsNullOrWhiteSpace(request.PromotionCode))
            throw new ArgumentException("Promotion code is required");

        // Get the booking
        if (!Guid.TryParse(request.BookingId, out var bookingId))
            throw new ArgumentException("Invalid booking ID format");

        var booking = await _bookingRepository.GetByIdAsync(bookingId,false);
        if (booking == null)
            throw new KeyNotFoundException($"Booking not found: {bookingId}");

        // Find promotion by code
        var allPromotions = await _promotionRepository.GetAll(false);
        var promotion = allPromotions.FirstOrDefault(p =>
            p.Code.Equals(request.PromotionCode, StringComparison.OrdinalIgnoreCase));

        if (promotion == null)
            throw new KeyNotFoundException($"Promotion not found with code: {request.PromotionCode}");

        // Store original total
        var originalTotal = booking.TotalPrice?.Amount ?? 0;

        // Apply promotion
        try
        {
            booking.ApplyPromotion(promotion);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Cannot apply promotion: {ex.Message}");
        }

        // Save the booking
        await _bookingRepository.SaveChangesAsync();

        var newTotal = booking.TotalPrice?.Amount ?? 0;
        var discountAmount = originalTotal - newTotal;

        return new ApplyPromotionToBookingResponse
        {
            BookingId = booking.Id.ToString(),
            PromotionCode = promotion.Code,
            OriginalTotal = originalTotal,
            DiscountAmount = discountAmount,
            NewTotal = newTotal,
            Message = $"Promotion '{promotion.Code}' successfully applied! Saving £{discountAmount:F2}"
        };
    }
}
