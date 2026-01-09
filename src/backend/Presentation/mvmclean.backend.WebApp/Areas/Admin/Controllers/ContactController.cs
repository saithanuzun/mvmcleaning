using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using mvmclean.backend.Application.Features.Contact.Commands;
using mvmclean.backend.Application.Features.Contact.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminCookie")]
[Route("admin/contacts")]
public class ContactController : Controller
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetAllContactsQuery());
        return View(result.Contacts);
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromForm] string fullName,
        [FromForm] string email,
        [FromForm] string? phoneNumber,
        [FromForm] string subject,
        [FromForm] string message)
    {
        var command = new CreateContactCommand
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            Subject = subject,
            Message = message
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            return RedirectToAction(nameof(Detail), new { id = result.ContactId });
        }

        ModelState.AddModelError("", result.Message);
        return View();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Detail(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetContactByIdQuery(id));
            return View(result.Contact);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Route("{id}/mark-read")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkContactAsReadCommand { ContactId = id });

        if (result.Success)
        {
            return RedirectToAction(nameof(Detail), new { id });
        }

        return NotFound();
    }

    [HttpPost]
    [Route("{id}/add-message")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMessage(
        Guid id,
        [FromForm] string message,
        [FromForm] string adminEmail)
    {
        var command = new AddContactMessageCommand
        {
            ContactId = id,
            Message = message,
            AdminEmail = adminEmail,
            IsAdminResponse = true
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            return RedirectToAction(nameof(Detail), new { id });
        }

        return NotFound();
    }

    [HttpPost]
    [Route("{id}/update-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromForm] string status)
    {
        var result = await _mediator.Send(new UpdateContactStatusCommand
        {
            ContactId = id,
            Status = status
        });

        if (result.Success)
        {
            return RedirectToAction(nameof(Detail), new { id });
        }

        return NotFound();
    }
}
