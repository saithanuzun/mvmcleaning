using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Services;


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
    /// Create a new booking with postcode and telephone number
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
            return Error("Invalid request data");

        try
        {
            var response = await _mediator.Send(request);
            return Success(response, "Booking created successfully");
        }
        catch (Exception ex)
        {
            return Error($"Error creating booking: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Get booking details by ID
    /// </summary>
    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingById(string bookingId)
    {
        if (string.IsNullOrEmpty(bookingId))
            return Error("Booking ID is required");

        try
        {
            if (!Guid.TryParse(bookingId, out var id))
                return Error("Invalid booking ID format");

            var request = new GetBookingByIdRequest
            {
                BookingId = bookingId
            };

            var response = await _mediator.Send(request);

            return Success(new
            {
                id = response.Id,
                phoneNumber = response.PhoneNumber,
                postcode = response.Postcode,
                contractorId = response.ContractorId,
                customerId = response.CustomerId,
                customer = response.Customer != null ? new
                {
                    id = response.Customer.Id,
                    name = response.Customer.FullName,
                    email = response.Customer.Email,
                    phoneNumber = response.Customer.PhoneNumber?.ToString()
                } : null,
                serviceItems = response.ServiceItems?.Select(item => new
                {
                    id = item.Id,
                    serviceName = item.ServiceName,
                    serviceId = item.ServiceId,
                    unitPrice = item.UnitAdjustedPrice?.Amount ?? 0,
                    quantity = item.Quantity,
                    totalPrice = (item.UnitAdjustedPrice?.Amount ?? 0) * item.Quantity
                }).ToList(),
                serviceAddress = response.ServiceAddress != null ? new
                {
                    street = response.ServiceAddress.Street,
                    city = response.ServiceAddress.City,
                    additionalInfo = response.ServiceAddress.AdditionalInfo
                } : null,
                scheduledSlot = response.ScheduledSlot != null ? new
                {
                    startTime = response.ScheduledSlot.StartTime,
                    endTime = response.ScheduledSlot.EndTime,
                    duration = (int)(response.ScheduledSlot.EndTime - response.ScheduledSlot.StartTime).TotalMinutes
                } : null,
                totalPrice = response.TotalPrice,
                currency = response.Currency,
                paymentId = response.PaymentId,
                status = response.Status.ToString(),
                creationStatus = response.CreationStatus.ToString(),
                createdAt = response.CreatedAt,
                updatedAt = response.UpdatedAt
            }, "Booking retrieved successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return Error(ex.Message, 404);
        }
        catch (Exception ex)
        {
            return Error($"Error retrieving booking: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Verify payment and confirm booking
    /// </summary>
    [HttpPost("verify-payment")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentAndConfirmBookingRequest request)
    {
        if (!ModelState.IsValid)
            return Error("Invalid request data");

        try
        {
            var response = await _mediator.Send(request);
            return Success(response, "Payment verified successfully");
        }
        catch (Exception ex)
        {
            return Error($"Error verifying payment: {ex.Message}", 500);
        }
    }
}
