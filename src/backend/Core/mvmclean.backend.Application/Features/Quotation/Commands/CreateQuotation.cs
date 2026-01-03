using MediatR;
using mvmclean.backend.Domain.Aggregates.Quotation;

namespace mvmclean.backend.Application.Features.Quotation.Commands;

public class CreateQuotationRequest : IRequest<CreateQuotationResponse>
{
    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }
}

public class CreateQuotationResponse 
{
    public CreateQuotationResponse(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}


public class CreateQuotationRequestHandler : IRequestHandler<CreateQuotationRequest,CreateQuotationResponse>
{
    private readonly IQuotationRepository _quotationRepository;

    public CreateQuotationRequestHandler(IQuotationRepository quotationRepository)
    {
        _quotationRepository = quotationRepository;
    }


    public async Task<CreateQuotationResponse> Handle(CreateQuotationRequest request, CancellationToken cancellationToken)
    {
        var quotation = Domain.Aggregates.Quotation.Quotation.Create(request.PhoneNumber, request.Postcode);
        
        
        await _quotationRepository.AddAsync(quotation);
        
        return new CreateQuotationResponse(quotation.Id.ToString());
    }
}
