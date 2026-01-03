using MediatR;
using mvmclean.backend.Domain.Aggregates.Quotation;
using mvmclean.backend.Domain.Aggregates.Quotation.Entities;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Quotation.Commands;

public class AddBasketRequest : IRequest<AddBasketResponse>
{
    public string QuotationId { get; set; }

    public string ServiceId { get; set; }
    public int Quantity { get; set; }
    public decimal Price   { get; set; }
}
public class AddBasketResponse
{
    public string QuotationId { get; set; }
}

public class AddBasketRequestHandler : IRequestHandler<AddBasketRequest, AddBasketResponse>
{
    private IQuotationRepository _quotationRepository;

    public AddBasketRequestHandler(IQuotationRepository quotationRepository)
    {
        _quotationRepository = quotationRepository;
    }


    public async Task<AddBasketResponse> Handle(AddBasketRequest request, CancellationToken cancellationToken)
    {
        var quote = await _quotationRepository.GetByIdAsync(Guid.Parse(request.QuotationId));
        
        quote.AddBasketItem(BasketItem.Create(Guid.Parse(request.ServiceId), Money.Create( request.Price), request.Quantity));
        
        return new AddBasketResponse(){QuotationId = request.QuotationId};
        
    }
}