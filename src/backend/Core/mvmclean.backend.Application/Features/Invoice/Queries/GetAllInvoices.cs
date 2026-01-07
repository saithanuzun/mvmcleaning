using MediatR;
using mvmclean.backend.Domain.Aggregates.Invoice;
using mvmclean.backend.Domain.Aggregates.Invoice.Enums;

namespace mvmclean.backend.Application.Features.Invoice.Queries;

public class GetAllInvoicesRequest : IRequest<GetAllInvoicesResponse>
{
}

public class GetAllInvoicesResponse
{
    public List<InvoiceDto> Invoices { get; set; } = new();
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public InvoiceStatus Status { get; set; }
    public int LineItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOverdue => Status != InvoiceStatus.Paid && DateTime.UtcNow > DueDate;
    public string StatusBadgeClass => Status switch
    {
        InvoiceStatus.Draft => "secondary",
        InvoiceStatus.Sent => "info",
        InvoiceStatus.Overdue => "danger",
        InvoiceStatus.Paid => "success",
        InvoiceStatus.Cancelled => "danger",
        _ => "secondary"
    };
}

public class GetAllInvoicesHandler : IRequestHandler<GetAllInvoicesRequest, GetAllInvoicesResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAllInvoicesHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<GetAllInvoicesResponse> Handle(GetAllInvoicesRequest request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAll(false);

        var invoiceDtos = invoices
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                BookingId = i.BookingId,
                CustomerId = i.CustomerId,
                IssueDate = i.IssueDate,
                DueDate = i.DueDate,
                PaidDate = i.PaidDate,
                Subtotal = i.Subtotal.Amount,
                DiscountAmount = i.DiscountAmount.Amount,
                TotalAmount = i.TotalAmount.Amount,
                Currency = i.TotalAmount.Currency,
                Status = i.Status,
                LineItemCount = i.LineItems.Count,
                CreatedAt = i.CreatedAt
            })
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

        return new GetAllInvoicesResponse { Invoices = invoiceDtos };
    }
}
