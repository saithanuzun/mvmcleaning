using MediatR;

namespace mvmclean.backend.Application.Features.Quotation.Queries;

public class GetAllQuotationsRequest : IRequest<GetAllQuotationsResponse>
{
    
}

public class GetAllQuotationsResponse 
{
    
}

public class GetAllQuotationsHandler : IRequestHandler<GetAllQuotationsRequest,GetAllQuotationsResponse> 
{
    public Task<GetAllQuotationsResponse> Handle(GetAllQuotationsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}