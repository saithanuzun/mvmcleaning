using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class ContractorController : BaseApiController
{
    public ContractorController(IMediator mediator) : base(mediator)
    {
        
    }

    [HttpGet("availability/day")]
    public async Task<IActionResult> GetContractorsAvailabilityByDay([FromQuery] GetContractorAvailabilityByDayRequest request)
    {
        var response = await _mediator.Send(request);
        return Success(response);
    }
    
    
}