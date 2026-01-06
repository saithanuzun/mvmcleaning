using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/bookings")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class BookingsController : BaseContractorController
{
    public BookingsController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetBookingsByContractorIdRequest { ContractorId = ContractorId.ToString() });
        return View(response);
    }

    [Route("details/{bookingId}")]
    public async Task<IActionResult> Details(Guid bookingId)
    {
        var response = await _mediator.Send(new GetBookingByIdRequest { BookingId = bookingId.ToString() });
        return View(response);
    }

    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        // Get all available services for the dropdown
        var services = await _mediator.Send(new GetAllServicesRequest());
        ViewBag.Services = services;
        ViewBag.Title = "Create Manual Booking";

        return View();
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(
        string phoneNumber,
        string postcode,
        string customerFirstName,
        string customerLastName,
        string? customerEmail,
        string? customerStreet,
        string? customerCity,
        string? customerAdditionalInfo,
        DateTime scheduledDateTime,
        int durationMinutes,
        List<string> serviceIds,
        List<string> serviceNames,
        List<decimal> servicePrices,
        List<int> serviceQuantities)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            // Build service items list
            var serviceItems = new List<ManualBookingServiceItem>();
            for (int i = 0; i < serviceIds.Count; i++)
            {
                if (!string.IsNullOrEmpty(serviceIds[i]) && serviceQuantities[i] > 0)
                {
                    serviceItems.Add(new ManualBookingServiceItem
                    {
                        ServiceId = serviceIds[i],
                        ServiceName = serviceNames[i],
                        Price = servicePrices[i],
                        Quantity = serviceQuantities[i]
                    });
                }
            }

            if (serviceItems.Count == 0)
            {
                TempData["Error"] = "Please add at least one service to the booking";
                var services = await _mediator.Send(new GetAllServicesRequest());
                ViewBag.Services = services;
                return View();
            }

            var request = new CreateManualBookingRequest
            {
                ContractorId = ContractorId.ToString()!,
                PhoneNumber = phoneNumber,
                Postcode = postcode,
                CustomerFirstName = customerFirstName,
                CustomerLastName = customerLastName,
                CustomerEmail = customerEmail,
                CustomerStreet = customerStreet,
                CustomerCity = customerCity,
                CustomerAdditionalInfo = customerAdditionalInfo,
                ScheduledDateTime = scheduledDateTime,
                DurationMinutes = durationMinutes,
                ServiceItems = serviceItems
            };

            var response = await _mediator.Send(request);

            TempData["Success"] = "Booking created successfully!";
            return RedirectToAction("Details", new { bookingId = response.BookingId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            var services = await _mediator.Send(new GetAllServicesRequest());
            ViewBag.Services = services;
            return View();
        }
    }
}

