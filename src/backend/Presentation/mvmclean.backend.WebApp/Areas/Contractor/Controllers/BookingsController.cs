using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/bookings")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class BookingsController : BaseContractorController
{
    public BookingsController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetBookingsByContractorIdRequest { ContractorId = ContractorId.ToString() });
        return View(response);
    }

    [Route("details/{bookingId}")]
    public async Task<IActionResult> Details(Guid bookingId)
    {
        var response = await _mediator.Send(new GetBookingByIdRequest { BookingId = bookingId.ToString() });
        return View(response);
    }
}
