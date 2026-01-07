using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Invoice;
using mvmclean.backend.Application.Features.Invoice.Commands;
using mvmclean.backend.Application.Features.Invoice.Queries;
using mvmclean.backend.Application.Features.Services;
using mvmclean.backend.Application.Features.Services.Commands;
using mvmclean.backend.Application.Features.Services.Queries;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/invoice")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class InvoiceController : BaseAdminController
{
    public InvoiceController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        return await AllInvoices();
    }

    [Route("all")]
    [HttpGet]
    public async Task<IActionResult> AllInvoices()
    {
        try
        {
            var response = await _mediator.Send(new GetAllInvoicesRequest());
            return View(response);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading invoices: {ex.Message}";
            return View(new GetAllInvoicesResponse());
        }
    }

    [Route("details/{invoiceId}")]
    [HttpGet]
    public async Task<IActionResult> Details(Guid invoiceId)
    {
        try
        {
            var response = await _mediator.Send(new GetInvoiceByIdRequest { InvoiceId = invoiceId });
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Invoice not found";
            return RedirectToAction(nameof(AllInvoices));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading invoice: {ex.Message}";
            return RedirectToAction(nameof(AllInvoices));
        }
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateInvoiceViewModel());
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateInvoiceRequest
            {
                BookingId = model.BookingId,
                PaymentTermsDays = model.PaymentTermsDays
            };

            var response = await _mediator.Send(request);

            if (response.Success)
            {
                TempData["Success"] = response.Message;
                return RedirectToAction(nameof(Details), new { invoiceId = response.InvoiceId });
            }
            else
            {
                TempData["Error"] = response.Message;
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error creating invoice: {ex.Message}";
            return View(model);
        }
    }

    [Route("mark-as-paid/{invoiceId}")]
    [HttpPost]
    public async Task<IActionResult> MarkAsPaid(Guid invoiceId)
    {
        try
        {
            var request = new MarkInvoiceAsPaidRequest
            {
                InvoiceId = invoiceId,
                PaymentDate = DateTime.UtcNow
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

            return RedirectToAction(nameof(Details), new { invoiceId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error marking invoice as paid: {ex.Message}";
            return RedirectToAction(nameof(Details), new { invoiceId });
        }
    }
}

public class CreateInvoiceViewModel
{
    public Guid BookingId { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
}