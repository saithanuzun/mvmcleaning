using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Commands;
using mvmclean.backend.Application.Features.Contractor.Queries;
using mvmclean.backend.Application.Features.Booking;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/availability")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class AvailabilityController : BaseContractorController
{
    public AvailabilityController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "My Availability";
        return View(response);
    }

    [Route("working-days")]
    [HttpGet]
    public async Task<IActionResult> WorkingDays()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Working Days";
        return View(response);
    }

    [Route("unavailability")]
    [HttpGet]
    public async Task<IActionResult> Unavailability()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        var response = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });
        ViewBag.Title = "Unavailability";
        return View(response);
    }

    [Route("unavailability/add")]
    [HttpPost]
    public async Task<IActionResult> AddUnavailability(DateTime startTime, DateTime endTime)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        if (startTime >= endTime)
        {
            TempData["Error"] = "Start time must be before end time";
            return RedirectToAction("Unavailability");
        }

        try
        {
            var request = new CreateUnavailabilityRequest
            {
                ContractorId = ContractorId.ToString(),
                StartTime = startTime,
                EndTime = endTime
            };

            var response = await _mediator.Send(request);
            TempData["Success"] = "Unavailability period has been added successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Unavailability");
    }

    [Route("unavailability/delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteUnavailability(DateTime startTime, DateTime endTime)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            var request = new DeleteUnavailabilityRequest
            {
                ContractorId = ContractorId.ToString(),
                StartTime = startTime,
                EndTime = endTime
            };

            var response = await _mediator.Send(request);
            TempData["Success"] = "Unavailability slot has been removed successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Unavailability");
    }

    [Route("working-days/add")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorkingDay(string dayOfWeek, string startTime, string endTime)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        if (string.IsNullOrWhiteSpace(startTime) || string.IsNullOrWhiteSpace(endTime))
        {
            TempData["Error"] = "Start time and end time are required";
            return RedirectToAction("WorkingDays");
        }

        if (!TimeOnly.TryParse(startTime, out var parsedStartTime))
        {
            TempData["Error"] = "Invalid start time format";
            return RedirectToAction("WorkingDays");
        }

        if (!TimeOnly.TryParse(endTime, out var parsedEndTime))
        {
            TempData["Error"] = "Invalid end time format";
            return RedirectToAction("WorkingDays");
        }

        if (parsedStartTime >= parsedEndTime)
        {
            TempData["Error"] = "Start time must be before end time";
            return RedirectToAction("WorkingDays");
        }

        try
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfWeek, out var parsedDay))
            {
                TempData["Error"] = "Invalid day of week";
                return RedirectToAction("WorkingDays");
            }

            var request = new SetWorkingDaysRequest
            {
                ContractorId = ContractorId.ToString(),
                DayOfWeek = parsedDay,
                StartTime = parsedStartTime,
                EndTime = parsedEndTime,
                IsWorkingDay = true
            };

            var response = await _mediator.Send(request);
            TempData["Success"] = $"Working hours for {parsedDay} have been set successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("WorkingDays");
    }

    [Route("working-days/delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWorkingDay(string dayOfWeek)
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        try
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfWeek, out var parsedDay))
            {
                TempData["Error"] = "Invalid day of week";
                return RedirectToAction("WorkingDays");
            }

            var request = new DeleteWorkingDayRequest
            {
                ContractorId = ContractorId.ToString(),
                DayOfWeek = parsedDay
            };

            var response = await _mediator.Send(request);

            if (response.Success)
            {
                TempData["Success"] = response.Message;
            }
            else
            {
                TempData["Error"] = response.Message;
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("WorkingDays");
    }

    [Route("calendar")]
    public IActionResult Calendar()
    {
        ViewBag.Title = "Availability Calendar";
        return View();
    }

    [Route("calendar/events")]
    [HttpGet]
    public async Task<IActionResult> GetCalendarEvents(DateTime start, DateTime end)
    {
        if (ContractorId == null) return Unauthorized();

        var events = new List<object>();

        // 1. Get Unavailable Slots
        var contractor = await _mediator.Send(new GetContractorByIdRequest { Id = ContractorId.ToString() });

        foreach (var slot in contractor.UnavailableSlots)
        {
            events.Add(new
            {
                id = $"unavailable-{slot.StartTime.Ticks}",
                title = "Unavailable",
                start = slot.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = slot.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                backgroundColor = "#dc3545", // red
                borderColor = "#dc3545",
                display = "block"
            });
        }

        // 2. Get Bookings
        var bookings = await _mediator.Send(new GetBookingsByContractorIdRequest { ContractorId = ContractorId.ToString() });

        foreach (var booking in bookings)
        {
            if (booking.ScheduledSlot != null)
            {
                events.Add(new
                {
                    id = $"booking-{booking.Id}",
                    title = $"Booking: {booking.CustomerName}",
                    start = booking.ScheduledSlot.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = booking.ScheduledSlot.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    backgroundColor = "#0d6efd", // blue
                    borderColor = "#0d6efd",
                    url = Url.Action("Details", "Bookings", new { bookingId = booking.Id })
                });
            }
        }

        // 3. Return Business Hours (Working Days)
        var businessHours = new List<object>();
        foreach (var workingDay in contractor.WorkingHours)
        {
            if (workingDay.IsWorkingDay)
            {
                businessHours.Add(new
                {
                    daysOfWeek = new[] { (int)workingDay.DayOfWeek },
                    startTime = workingDay.StartTime.ToString("HH:mm"),
                    endTime = workingDay.EndTime.ToString("HH:mm")
                });
            }
        }

        return Json(new { events, businessHours });
    }
}

