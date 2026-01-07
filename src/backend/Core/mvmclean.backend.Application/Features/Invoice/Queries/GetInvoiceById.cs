using MediatR;

namespace mvmclean.backend.Application.Features.Invoice.Queries;

public class GetInvoiceByIdRequest : IRequest<GetInvoiceByIdResponse>
{
    public Guid InvoiceId { get; set; }
}

public class GetInvoiceByIdResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
}

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdRequest, GetInvoiceByIdResponse>
{
    public async Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement invoice repository queries
        // For now, return default response
        return await Task.FromResult(new GetInvoiceByIdResponse());
    }
}
