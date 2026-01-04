using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class ContractorController : BaseApiController
{
    public ContractorController(IMediator mediator) : base(mediator)
    {
        
    }

    public async Task<IActionResult> GetContractorAvailabilityByDay(GetContractorAvailabilityByDayRequest request)
    {

        var respones = await _mediator.Send(request);
        
        
        return Ok(respones);
    }
}