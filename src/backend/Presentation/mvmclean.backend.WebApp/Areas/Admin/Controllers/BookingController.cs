using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Services;

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
            return View(new GetAllBookingsRequest());
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
}