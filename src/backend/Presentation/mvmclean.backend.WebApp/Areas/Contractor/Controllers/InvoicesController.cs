using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor/invoices")]
[Authorize(AuthenticationSchemes = "ContractorCookie")]
public class InvoicesController : BaseContractorController
{
    public InvoicesController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        if (ContractorId == null)
            return RedirectToAction("Index", "Home");

        // TODO: Implement GetInvoicesByContractorId in Application layer
        return View();
    }

    [Route("details/{invoiceId}")]
    public IActionResult Details(Guid invoiceId)
    {
        // TODO: Implement GetInvoiceById in Application layer
        return View();
    }
}
