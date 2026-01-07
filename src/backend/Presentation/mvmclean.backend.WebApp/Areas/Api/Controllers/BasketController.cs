using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking.Commands;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class BasketController : BaseApiController
{
    public BasketController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Add a service to booking cart
    /// </summary>
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddCartItemRequest request)
    {
        if (!ModelState.IsValid)
            return Error("Invalid request data");

        try
        {
            var response = await _mediator.Send(request);

            if (!response.Success)
                return Error(response.Message);

            return Success(new 
            { 
                bookingId = response.BookingId,
                totalPrice = response.TotalPrice,
                totalDurationMinutes = response.TotalDurationMinutes
            }, response.Message);
        }
        catch (Exception ex)
        {
            return Error($"Error adding to cart: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Remove a service from booking cart
    /// </summary>
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartItemRequest request)
    {
        if (!ModelState.IsValid)
            return Error("Invalid request data");

        try
        {
            var response = await _mediator.Send(request);

            if (!response.Success)
                return Error(response.Message);

            return Success(new 
            { 
                bookingId = response.BookingId,
                totalPrice = response.TotalPrice,
                totalDurationMinutes = response.TotalDurationMinutes
            }, response.Message);
        }
        catch (Exception ex)
        {
            return Error($"Error removing from cart: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Get basket/cart for a booking
    /// </summary>
    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBasket(string bookingId)
    {
        if (string.IsNullOrEmpty(bookingId))
            return Error("Booking ID is required");

        try
        {
            if (!Guid.TryParse(bookingId, out var id))
                return Error("Invalid booking ID format");

            // You can create a GetBookingBasketRequest if needed
            // For now, returning a placeholder with basic structure
            return Success(new 
            { 
                bookingId = bookingId,
                items = new List<object>(),
                totalAmount = 0m
            }, "Basket retrieved");
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving basket: {ex.Message}", 500);
        }
    }
}
