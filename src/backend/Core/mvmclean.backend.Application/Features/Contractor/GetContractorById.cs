using MediatR;

namespace mvmclean.backend.Application.Features.Contractor;

public class GetContractorByIdRequest : IRequest<GetContractorByIdResponse>
{
    
}

public class GetContractorByIdResponse
{
    public string FullName { get; set; }
    //Todo
}

public class GetContractorByIdHandler : IRequestHandler<GetContractorByIdRequest,GetContractorByIdResponse>
{
    public Task<GetContractorByIdResponse> Handle(GetContractorByIdRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}