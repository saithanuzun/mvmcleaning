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
        try
        {
            var response = await _mediator.Send(new GetServiceByIdRequest {ServiceId = serviceId});
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Service not found.";
            return RedirectToAction("AllServices");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading service details: {ex.Message}";
            return RedirectToAction("AllServices");
        }
    }
    
    [Route("AllServices")]
    public async Task<IActionResult> AllServices()
    {
        try
        {
            var response = await _mediator.Send(new GetAllServicesRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading services: {ex.Message}";
            return View(new List<GetAllServicesResponse>());
        }
    }
    
    
    [Route("CreateService")]
    public IActionResult CreateService()
    {
        return View(new CreateServiceRequest());
    }
    
    [Route("CreateService")]
    [HttpPost]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            
            var response = await _mediator.Send(request);
            TempData["Success"] = "Service created successfully!";
            return RedirectToAction("ServiceDetails", new {serviceId = response.ServiceId});
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating service: {ex.Message}";
            return View(request);
        }
    }

    [Route("ServicePricings")]
    public async Task<IActionResult> ServicePricingsDetails(Guid serviceId)
    {
        try
        {
            var response = await _mediator.Send(new GetServicePostcodePricingsRequest { ServiceId = serviceId.ToString() });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Service not found.";
            return RedirectToAction("AllServices");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading service pricing: {ex.Message}";
            return RedirectToAction("AllServices");
        }
    }
    
    [Route("CreateServicePricings")]
    [HttpPost]
    public async Task<IActionResult> CreateServicePricing(AddServicePostcodePricingRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            
            var response = await _mediator.Send(request);
            TempData["Success"] = "Service pricing rule added successfully!";
            return RedirectToAction("ServicePricings", new {serviceId = response.ServiceId});
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating service pricing: {ex.Message}";
            return View(request);
        }
    }
    
    [Route("CreateServicePricings")]
    public IActionResult CreateServicePricing()
    {
        return View(new AddServicePostcodePricingRequest());
    }
    
   
    
}