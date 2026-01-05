using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class ContractorController : BaseApiController
{
    public ContractorController(IMediator mediator) : base(mediator)
    {
        
    }

    public async Task<IActionResult> GetContractorsAvailabilityByDay(GetContractorAvailabilityByDayRequest request)
    {

        var response = await _mediator.Send(request);
        
        
        return Ok(response);
    }
    
    
}