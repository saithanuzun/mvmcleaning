using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Invoice;
using mvmclean.backend.Domain.Aggregates.Invoice.Enums;

namespace mvmclean.backend.Application.Features.Invoice.Queries;

public class GetInvoiceByIdRequest : IRequest<GetInvoiceByIdResponse>
{
    public Guid InvoiceId { get; set; }
}

public class GetInvoiceByIdResponse
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
    public string Currency { get; set; } = "GBP";
    public InvoiceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
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

public class InvoiceLineItemDto
{
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdRequest, GetInvoiceByIdResponse>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        
        if (invoice == null)
        {
            throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found");
        }

        return new GetInvoiceByIdResponse
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            BookingId = invoice.BookingId,
            CustomerId = invoice.CustomerId,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
            Subtotal = invoice.Subtotal.Amount,
            DiscountAmount = invoice.DiscountAmount.Amount,
            TotalAmount = invoice.TotalAmount.Amount,
            Status = invoice.Status,
            CreatedAt = invoice.CreatedAt,
            LineItems = invoice.LineItems.Select(li => new InvoiceLineItemDto
            {
                Description = li.Description,
                UnitPrice = li.UnitPrice.Amount,
                Quantity = li.Quantity
            }).ToList()
        };
    }
}
