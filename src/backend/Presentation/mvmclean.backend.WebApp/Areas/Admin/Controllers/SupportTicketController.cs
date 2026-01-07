using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.SupportTicket;
using mvmclean.backend.Application.Features.SupportTicket.Commands;
using mvmclean.backend.Application.Features.SupportTicket.Queries;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/SupportTicket")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class SupportTicketController : BaseAdminController
{
    public SupportTicketController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllTickets();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllTickets()
    {
        try
        {
            var response = await _mediator.Send(new GetAllSupportTicketsRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading support tickets: {ex.Message}";
            return View(new GetAllSupportTicketsResponse());
        }
    }

    [Route("answer/{ticketId}")]
    [HttpGet]
    public async Task<IActionResult> Answer(Guid ticketId)
    {
        try
        {
            var response = await _mediator.Send(new GetSupportTicketByIdRequest { TicketId = ticketId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Support ticket not found";
            return RedirectToAction(nameof(AllTickets));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading support ticket: {ex.Message}";
            return RedirectToAction(nameof(AllTickets));
        }
    }

    [Route("details/{ticketId}")]
    [HttpGet]
    public async Task<IActionResult> Details(Guid ticketId)
    {
        try
        {
            var response = await _mediator.Send(new GetSupportTicketByIdRequest { TicketId = ticketId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Support ticket not found";
            return RedirectToAction(nameof(AllTickets));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading support ticket: {ex.Message}";
            return RedirectToAction(nameof(AllTickets));
        }
    }

    [Route("add-message/{ticketId}")]
    [HttpPost]
    public async Task<IActionResult> AddMessage(Guid ticketId, [FromForm] string message, [FromForm] bool isInternalNote = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message cannot be empty";
                return RedirectToAction(nameof(Details), new { ticketId });
            }

            var adminId = User.FindFirst("UserId")?.Value;
            if (!Guid.TryParse(adminId, out var senderId))
            {
                TempData["Error"] = "Unable to identify current user";
                return RedirectToAction(nameof(Details), new { ticketId });
            }

            var response = await _mediator.Send(new AddMessageToTicketRequest
            {
                TicketId = ticketId,
                SenderId = senderId,
                Message = message,
                IsInternalNote = isInternalNote
            });

            if (response.Success)
            {
                TempData["Success"] = "Message added successfully";
            }

            return RedirectToAction(nameof(Details), new { ticketId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Support ticket not found";
            return RedirectToAction(nameof(AllTickets));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error adding message: {ex.Message}";
            return RedirectToAction(nameof(Details), new { ticketId });
        }
    }

    [Route("assign/{ticketId}")]
    [HttpPost]
    public async Task<IActionResult> Assign(Guid ticketId, [FromForm] Guid contractorId)
    {
        try
        {
            var response = await _mediator.Send(new AssignTicketRequest
            {
                TicketId = ticketId,
                ContractorId = contractorId
            });

            if (response.Success)
            {
                TempData["Success"] = "Ticket assigned successfully";
            }

            return RedirectToAction(nameof(Details), new { ticketId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Support ticket not found";
            return RedirectToAction(nameof(AllTickets));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error assigning ticket: {ex.Message}";
            return RedirectToAction(nameof(Details), new { ticketId });
        }
    }

    [Route("close/{ticketId}")]
    [HttpPost]
    public async Task<IActionResult> Close(Guid ticketId, [FromForm] string resolution)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(resolution))
            {
                TempData["Error"] = "Resolution cannot be empty";
                return RedirectToAction(nameof(Details), new { ticketId });
            }

            var response = await _mediator.Send(new CloseTicketRequest
            {
                TicketId = ticketId,
                Resolution = resolution
            });

            if (response.Success)
            {
                TempData["Success"] = "Ticket closed successfully";
                return RedirectToAction(nameof(AllTickets));
            }

            return RedirectToAction(nameof(Details), new { ticketId });
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Support ticket not found";
            return RedirectToAction(nameof(AllTickets));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error closing ticket: {ex.Message}";
            return RedirectToAction(nameof(Details), new { ticketId });
        }
    }
}