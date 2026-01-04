using MediatR;

namespace mvmclean.backend.Application.Features.Booking;

public class GetAllBookingsRequest : IRequest<GetAllBookingsResponse>
{
    
}

public class GetAllBookingsResponse
{
    
}

public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsRequest,GetAllBookingsResponse>
{
    public Task<GetAllBookingsResponse> Handle(GetAllBookingsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}