using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.WebApp.Areas.Contractor.Models;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor")] 
[Authorize(AuthenticationSchemes = "ContractorCookie")] 
public class HomeController : BaseContractorController
{
    public HomeController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var request = new GetContractorByIdRequest() { Id = ContractorId.ToString() };
        
        var response = await _mediator.Send(request);
        
        var contractorViewModel = new ContractorViewModel
        {
            FullName = response.FullName,
            Username = response.Username,
            Email = response.Email,
            PhoneNumber = response.PhoneNumber,
        };

        
        return View(contractorViewModel);
    }
    
    
    
}