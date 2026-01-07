using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/booking")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class BookingController : BaseAdminController
{
    public BookingController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllBookings();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllBookings()
    {
        try
        {
            var response = await _mediator.Send(new GetAllBookingsRequest());
            return View(response);
        }
        catch (Exception ex)
        {   
            TempData["Error"] = $"Error loading bookings: {ex.Message}";
            return View();
        }
    }

    [Route("details/{bookingId}")]
    [HttpGet]
    public async Task<IActionResult> Details(string bookingId)
    {
        try
        {
            var response = await _mediator.Send(new GetBookingByIdRequest { BookingId = bookingId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Booking not found";
            return RedirectToAction(nameof(AllBookings));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading booking: {ex.Message}";
            return RedirectToAction(nameof(AllBookings));
        }
    }

    [Route("update-status/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string bookingId, [FromForm] string status)
    {
        try
        {
            var response = await _mediator.Send(new UpdateBookingStatusRequest
            {
                BookingId = bookingId,
                Status = status
            });

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Details), new { bookingId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Booking not found";
            return RedirectToAction(nameof(AllBookings));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating booking status: {ex.Message}";
            return RedirectToAction(nameof(Details), new { bookingId });
        }
    }

    [Route("confirm/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(string bookingId)
    {
        return await UpdateStatus(bookingId, "confirmed");
    }

    [Route("complete/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(string bookingId)
    {
        return await UpdateStatus(bookingId, "completed");
    }

    [Route("cancel/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(string bookingId)
    {
        return await UpdateStatus(bookingId, "cancelled");
    }
}