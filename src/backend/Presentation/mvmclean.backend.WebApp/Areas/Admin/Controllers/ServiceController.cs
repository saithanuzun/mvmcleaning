using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

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
    [HttpGet]
    public IActionResult CreateService()
    {
        return View(new CreateServiceRequest());
    }

    [Route("CreateService")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    TempData["Error"] = $"Validation error: {error.ErrorMessage}";
                }
                return View(request);
            }

            var response = await _mediator.Send(request);
            TempData["Success"] = "Service created successfully!";
            return RedirectToAction("AllServices");
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

    [Route("edit/{serviceId}")]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid serviceId)
    {
        try
        {
            var service = await _mediator.Send(new GetServiceByIdRequest { ServiceId = serviceId });
            var request = new UpdateServiceRequest
            {
                ServiceId = serviceId,
                Name = service.Name,
                Description = service.Description,
                Shortcut = service.Shortcut,
                BasePrice = service.BasePrice,
                EstimatedDurationMinutes = Convert.ToInt32(service.Duration)
            };
            return View(request);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Service not found";
            return RedirectToAction("AllServices");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading service: {ex.Message}";
            return RedirectToAction("AllServices");
        }
    }

    [Route("edit/{serviceId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateServiceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var response = await _mediator.Send(request);
            TempData["Success"] = response.Message;
            return RedirectToAction("ServiceDetails", new { serviceId = request.ServiceId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Service not found";
            return RedirectToAction("AllServices");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating service: {ex.Message}";
            return View(request);
        }
    }

    [Route("delete/{serviceId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid serviceId)
    {
        try
        {
            var response = await _mediator.Send(new DeleteServiceRequest { ServiceId = serviceId });
            TempData["Success"] = response.Message;
            return RedirectToAction("AllServices");
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Service not found";
            return RedirectToAction("AllServices");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting service: {ex.Message}";
            return RedirectToAction("AllServices");
        }
    }
}