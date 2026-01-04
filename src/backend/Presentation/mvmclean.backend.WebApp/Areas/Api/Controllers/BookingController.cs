using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class BookingController : BaseApiController
{
    public BookingController(IMediator mediator) : base(mediator)
    {
        
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> AddCartItem(CreateBookingRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    
    
}