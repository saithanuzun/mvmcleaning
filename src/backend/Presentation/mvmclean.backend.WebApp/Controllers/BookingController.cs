using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking.Queries;

namespace mvmclean.backend.WebApp.Controllers;

public class BookingController : Controller
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Display the search/find booking page
    /// </summary>
    [HttpGet]
    public IActionResult Find()
    {
        return View();
    }

    /// <summary>
    /// Search for booking by phone and postcode
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Find(string phoneNumber, string postcode)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(postcode))
        {
            ModelState.AddModelError("", "Please enter both phone number and postcode");
            return View();
        }

        try
        {
            var request = new GetBookingByPhoneAndPostcodeRequest
            {
                PhoneNumber = phoneNumber,
                Postcode = postcode
            };

            var booking = await _mediator.Send(request);
            
            // Redirect to details page with booking ID
            return RedirectToAction(nameof(Details), new { id = booking.Id });
        }
        catch (KeyNotFoundException)
        {
            ModelState.AddModelError("", "No booking found with the provided phone number and postcode");
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            return View();
        }
    }

    /// <summary>
    /// Display booking details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        try
        {
            var request = new GetBookingByIdRequest { BookingId = id };
            var booking = await _mediator.Send(request);
            return View(booking);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            return View();
        }
    }
}
