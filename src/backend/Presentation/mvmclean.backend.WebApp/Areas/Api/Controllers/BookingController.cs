using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Contractor;

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
    public async Task<IActionResult> AddCartItem(AddCartItemRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetContractorsByPostcode(GetContractorsByPostcodeRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> AssignContractor(AssignContractorRequest request)
    {

        var response = await _mediator.Send(request);
        
        
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> AssignCustomer(AssignCustomerRequest request)
    {

        var response = await _mediator.Send(request);
        
        
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> AssignPayment(AssignPaymentRequest request)
    {

        var response = await _mediator.Send(request);
        
        
        return Ok(response);
    }
    
    
    
    
    
}