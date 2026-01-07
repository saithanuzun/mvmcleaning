using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Services;
using mvmclean.backend.WebApp.Areas.Api.Models;
using AppCommands = mvmclean.backend.Application.Features.Booking.Commands;
using AppQueries = mvmclean.backend.Application.Features.Booking.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class BookingController : BaseApiController
{
    private readonly IStripeService _stripeService;

    public BookingController(
        IMediator mediator,
        IStripeService stripeService) : base(mediator)
    {
        _stripeService = stripeService;
    }

    /// <summary>
    /// Creates a new booking and generates payment link
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingApiRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                return Error("Customer name is required");

            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                return Error("Customer email is required");

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                return Error("Customer phone is required");

            if (string.IsNullOrWhiteSpace(request.Address))
                return Error("Address is required");

            if (string.IsNullOrWhiteSpace(request.Postcode))
                return Error("Postcode is required");

            if (request.ScheduledSlot == null)
                return Error("Scheduled slot is required");

            if (!request.Services.Any())
                return Error("At least one service is required");

            // Create booking
            var createBookingRequest = new AppCommands.CreateBookingCompleteRequest
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                Address = request.Address,
                Postcode = request.Postcode,
                ContractorId = request.ContractorId.ToString(),
                ScheduledStartTime = request.ScheduledSlot.StartTime,
                ScheduledEndTime = request.ScheduledSlot.EndTime,
                ServiceItems = request.Services.Select(s => new AppCommands.CreateBookingCompleteRequest.ServiceItemDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Quantity = s.Quantity,
                    Price = s.Price
                }).ToList(),
                TotalAmount = request.TotalAmount
            };

            var bookingResponse = await _mediator.Send(createBookingRequest);

            if (!bookingResponse.Success)
            {
                return Error(bookingResponse.Message);
            }

            // Create Stripe payment link
            var successUrl = $"{Request.Scheme}://{Request.Host}/shop/payment-success?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/shop/postcode";

            var paymentLink = await _stripeService.CreatePaymentLinkAsync(
                bookingResponse.BookingId,
                request.TotalAmount,
                "gbp",
                $"Booking #{bookingResponse.BookingId.ToString().Substring(0, 8)} - {request.CustomerName}",
                successUrl,
                cancelUrl);

            return Success(new Models.CreateBookingResponse
            {
                BookingId = bookingResponse.BookingId,
                PaymentUrl = paymentLink,
                Message = "Booking created successfully"
            });
        }
        catch (Exception ex)
        {
            return Error($"Error creating booking: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Gets booking details by ID
    /// </summary>
    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBooking(Guid bookingId)
    {
        try
        {
            var booking = await _mediator.Send(new AppQueries.GetBookingByIdRequest { BookingId = bookingId.ToString() });

            if (booking == null)
            {
                return Error("Booking not found", 404);
            }

            return Success(new BookingDetailsResponse
            {
                Id = booking.Id,
                CustomerName = booking.Customer?.FullName ?? "Unknown",
                CustomerEmail = booking.Customer?.Email?.Value ?? "",
                CustomerPhone = booking.PhoneNumber,
                Address = booking.ServiceAddress?.Street ?? "",
                Postcode = booking.Postcode,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalPrice,
                ScheduledSlot = booking.ScheduledSlot != null ? new ScheduledSlotDto
                {
                    StartTime = booking.ScheduledSlot.StartTime,
                    EndTime = booking.ScheduledSlot.EndTime
                } : null,
                Services = booking.ServiceItems?.Select(s => new ServiceItemDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Quantity = s.Quantity,
                    Price = s.UnitAdjustedPrice.Amount
                }).ToList(),
                CreatedAt = booking.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Error($"Error fetching booking: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Verifies payment and confirms booking
    /// </summary>
    [HttpPost("verify-payment")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        try
        {
            var result = await _mediator.Send(new AppCommands.VerifyPaymentAndConfirmBookingRequest
            {
                BookingId = request.BookingId,
                SessionId = request.SessionId
            });

            if (!result.Success)
            {
                return Error(result.Message);
            }

            return Success(new
            {
                bookingId = result.BookingId,
                status = result.Status,
                message = result.Message
            });
        }
        catch (Exception ex)
        {
            return Error($"Error verifying payment: {ex.Message}", 500);
        }
    }
}
