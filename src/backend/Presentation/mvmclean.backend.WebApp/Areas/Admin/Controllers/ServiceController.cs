using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Service")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class ServiceController : BaseAdminController
{
    public ServiceController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
    
    [Route("ServiceDetails/{serviceId}")]
    public async Task<IActionResult> ServiceDetails(Guid serviceId)
    {
        var response = await _mediator.Send(new GetServiceByIdRequest{ServiceId = serviceId});
        
        return View(response);
    }
    
    [Route("AllServices")]
    public async Task<IActionResult> AllServices()
    {
        var response = await _mediator.Send(new GetAllServicesRequest());
        return View(response);
    }
    
    
    [Route("CreateService")]
    public IActionResult CreateService()
    {
        return View();
    }
    
    [Route("CreateService")]
    [HttpPost]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        var response = await _mediator.Send(request);
        return RedirectToAction("ServiceDetails", new {serviceId = response.ServiceId});
    }

    [Route("ServicePricings")]
    public async Task<IActionResult> ServicePricingsDetails(Guid serviceId)
    {
        var response =await _mediator.Send(new GetServicePostcodePricingsRequest{ ServiceId = serviceId.ToString()});
        return View(response);
    }
    
    
    [Route("CreateServicePricings")]
    [HttpPost]
    public async Task<IActionResult> CreateServicePricing(AddServicePostcodePricingRequest request)
    {
        var response =await  _mediator.Send(request);
        return RedirectToAction("ServicePricings", new {serviceId = response.ServiceId});
    }
    
    [Route("CreateServicePricings")]
    public IActionResult CreateServicePricing()
    {
        return View();
    }
    
   
    
}