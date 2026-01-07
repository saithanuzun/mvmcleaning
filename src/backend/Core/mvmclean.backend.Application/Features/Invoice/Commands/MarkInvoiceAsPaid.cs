using MediatR;
using mvmclean.backend.Domain.Aggregates.Invoice;

namespace mvmclean.backend.Application.Features.Invoice.Commands;

public class MarkInvoiceAsPaidRequest : IRequest<MarkInvoiceAsPaidResponse>
{
    public Guid InvoiceId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}

public class MarkInvoiceAsPaidResponse
{
    public Guid InvoiceId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class MarkInvoiceAsPaidHandler : IRequestHandler<MarkInvoiceAsPaidRequest, MarkInvoiceAsPaidResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public MarkInvoiceAsPaidHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<MarkInvoiceAsPaidResponse> Handle(MarkInvoiceAsPaidRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        
        if (invoice == null)
        {
            return new MarkInvoiceAsPaidResponse
            {
                Success = false,
                Message = "Invoice not found"
            };
        }

        try
        {
            invoice.MarkAsPaid(request.PaymentDate);
            await _invoiceRepository.UpdateAsync(invoice);

            return new MarkInvoiceAsPaidResponse
            {
                InvoiceId = invoice.Id,
                Message = "Invoice marked as paid successfully",
                Success = true
            };
        }
        catch (InvalidOperationException ex)
        {
            return new MarkInvoiceAsPaidResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
