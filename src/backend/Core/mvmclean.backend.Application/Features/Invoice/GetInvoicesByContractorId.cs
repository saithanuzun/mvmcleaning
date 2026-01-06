using MediatR;

namespace mvmclean.backend.Application.Features.Invoice;

public class GetInvoicesByContractorIdRequest : IRequest<List<GetInvoicesByContractorIdResponse>>
{
    public string ContractorId { get; set; }
}

public class GetInvoicesByContractorIdResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
}

public class GetInvoicesByContractorIdHandler : IRequestHandler<GetInvoicesByContractorIdRequest, List<GetInvoicesByContractorIdResponse>>
{
    public async Task<List<GetInvoicesByContractorIdResponse>> Handle(GetInvoicesByContractorIdRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement invoice repository queries
        // For now, return empty list
        return await Task.FromResult(new List<GetInvoicesByContractorIdResponse>());
    }
}
