using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Invoice;
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
}