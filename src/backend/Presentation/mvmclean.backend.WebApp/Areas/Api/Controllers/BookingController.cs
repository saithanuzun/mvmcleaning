using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

public class BookingController : BaseApiController
{
    public BookingController(IMediator mediator) : base(mediator)
    {
        
    }

    public IActionResult CreateBooking()
    {
        return Ok();
    }
    
}