using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services.Queries;
using mvmclean.backend.WebApp.Areas.Api.Models;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class ServicesController : BaseApiController
{
    public ServicesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Gets all available services for a specific postcode
    /// </summary>
    [HttpGet("bypostcode/{postcode}")]
    public async Task<IActionResult> GetServicesByPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return Error("Postcode is required");
        }

        try
        {
            var services = await _mediator.Send(new GetServicesByPostcodeRequest { Postcode = postcode });
            
            var response = services.Select(s => new ServiceResponse
            {
                Id = s.ContractorId,
                Name = s.Name,
                Description = s.Description,
                Category = s.Category,
                Duration = s.Duration,
                BasePrice = s.Price,
                PostcodePrice = s.Price
            }).ToList();

            return Success(response);
        }
        catch (Exception ex)
        {
            return Error($"Error fetching services: {ex.Message}", 500);
        }
    }

    /// <summary>
    /// Gets service details by ID
    /// </summary>
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetServiceById(Guid serviceId)
    {
        try
        {
            var service = await _mediator.Send(new GetServiceByIdRequest { ServiceId = serviceId });
            
            if (service == null)
            {
                return Error("Service not found", 404);
            }

            var response = new ServiceResponse
            {
                Id = service.ServiceId,
                Name = service.Name,
                Description = service.Description,
                Category = service.CategoryName,
                Duration = int.TryParse(service.Duration, out var duration) ? duration : 0,
                BasePrice = service.BasePrice
            };

            return Success(response);
        }
        catch (Exception ex)
        {
            return Error($"Error fetching service: {ex.Message}", 500);
        }
    }
}
