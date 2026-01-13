using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Features.Services.Queries;
using mvmclean.backend.Application.Features.Contractor.Queries;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.WebApp.Areas.Admin.Models;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Booking")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class BookingController : BaseAdminController
{
    public BookingController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllBookings();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllBookings()
    {
        try
        {
            var response = await _mediator.Send(new GetAllBookingsRequest());
            return View(response.Where(i=>i.PhoneNumber!="07862265412").OrderByDescending(i=>i.CreatedAt).ToList());
        }
        catch (Exception ex)
        {   
            TempData["Error"] = $"Error loading bookings: {ex.Message}";
            return View(new List<GetAllBookingsResponse>());
        }
    }

    [Route("details/{bookingId}")]
    [HttpGet]
    public async Task<IActionResult> Details(string bookingId)
    {
        try
        {
            var response = await _mediator.Send(new GetBookingByIdRequest { BookingId = bookingId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Booking not found";
            return RedirectToAction(nameof(AllBookings));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading booking: {ex.Message}";
            return RedirectToAction(nameof(AllBookings));
        }
    }

    [Route("update-status/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string bookingId, [FromForm] string status)
    {
        try
        {
            var response = await _mediator.Send(new UpdateBookingStatusRequest
            {
                BookingId = bookingId,
                Status = status
            });

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Details), new { bookingId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Booking not found";
            return RedirectToAction(nameof(AllBookings));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating booking status: {ex.Message}";
            return RedirectToAction(nameof(Details), new { bookingId });
        }
    }

    [Route("confirm/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(string bookingId)
    {
        return await UpdateStatus(bookingId, "confirmed");
    }

    [Route("complete/{bookingId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(string bookingId)
    {
        return await UpdateStatus(bookingId, "completed");
    }

    [Route("cancel/{bookingId}")]
    [HttpPost]
    public async Task<IActionResult> Cancel(string bookingId)
    {
        return await UpdateStatus(bookingId, "cancelled");
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var servicesResponse = await _mediator.Send(new GetAllServicesRequest());
            var contractorsResponse = await _mediator.Send(new GetAllContractorsRequest());
            
            ViewBag.Services = servicesResponse;
            ViewBag.Contractors = contractorsResponse.Contractors;
            
            return View();
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading form data: {ex.Message}";
            return RedirectToAction(nameof(AllBookings));
        }
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateBookingFormModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var servicesResponse = await _mediator.Send(new GetAllServicesRequest());
                var contractorsResponse = await _mediator.Send(new GetAllContractorsRequest());
                
                ViewBag.Services = servicesResponse;
                ViewBag.Contractors = contractorsResponse.Contractors;
                
                return View(model);
            }

            // Parse service items from form
            var serviceItems = new List<CreateBookingCompleteRequest.ServiceItemDto>();
            var serviceIds = model.ServiceIds ?? new List<string>();
            var serviceNames = model.ServiceNames ?? new List<string>();
            var servicePrices = model.ServicePrices ?? new List<string>();
            var serviceQuantities = model.ServiceQuantities ?? new List<int>();

            for (int i = 0; i < serviceIds.Count; i++)
            {
                if (Guid.TryParse(serviceIds[i], out var serviceId) && 
                    decimal.TryParse(servicePrices[i], out var price) &&
                    i < serviceQuantities.Count && serviceQuantities[i] > 0)
                {
                    serviceItems.Add(new CreateBookingCompleteRequest.ServiceItemDto
                    {
                        ServiceId = serviceId,
                        ServiceName = serviceNames[i],
                        Price = price,
                        Quantity = serviceQuantities[i]
                    });
                }
            }

            if (!serviceItems.Any())
            {
                ModelState.AddModelError("", "Please select at least one service");
                var servicesResponse = await _mediator.Send(new GetAllServicesRequest());
                var contractorsResponse = await _mediator.Send(new GetAllContractorsRequest());
                
                ViewBag.Services = servicesResponse;
                ViewBag.Contractors = contractorsResponse.Contractors;
                
                return View(model);
            }

            // Create booking request
            var request = new CreateBookingCompleteRequest
            {
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                Address = model.Address,
                Postcode = model.Postcode,
                ContractorId = model.ContractorId,
                ScheduledStartTime = DateTime.Parse($"{model.ScheduledDate} {model.ScheduledTime}"),
                ScheduledEndTime = DateTime.Parse($"{model.ScheduledDate} {model.ScheduledTime}").AddMinutes(model.DurationMinutes),
                TotalAmount = model.TotalAmount,
                ServiceItems = serviceItems
            };

            var response = await _mediator.Send(request);

            if (response.Success)
            {
                TempData["Success"] = "Booking created successfully";
                return RedirectToAction("AllBookings");
            }
            else
            {
                TempData["Error"] = response.Message;
                var servicesResponse = await _mediator.Send(new GetAllServicesRequest());
                var contractorsResponse = await _mediator.Send(new GetAllContractorsRequest());
                
                ViewBag.Services = servicesResponse;
                ViewBag.Contractors = contractorsResponse.Contractors;
                
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating booking: {ex.Message}";
            return RedirectToAction("AllBookings");
        }
    }

    [Route("activate-contractor/{contractorId}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateContractor(string contractorId)
    {
        try
        {
            var response = await _mediator.Send(new UpdateContractorStatusRequest
            {
                ContractorId = contractorId,
                IsActive = true
            });

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }

            return RedirectToAction(nameof(Create));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error activating contractor: {ex.Message}";
            return RedirectToAction(nameof(Create));
        }
    }
}

