using MediatR;

namespace mvmclean.backend.Application.Features.Quotation.Commands;

public class CreateQuotationRequest : IRequest<CreateQuotationResponse>
{
    
}

public class CreateQuotationResponse 
{
    
}


public class CreateQuotationRequestHandler : IRequestHandler<CreateQuotationRequest,CreateQuotationResponse>
{
    public Task<CreateQuotationResponse> Handle(CreateQuotationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
