using MediatR;
using mvmclean.backend.Application.Features.Booking.Commands;

namespace mvmclean.backend.Application.Features.Booking.Queries;

public class GetAllBookingsRequest : IRequest<GetAllBookingsResponse>
{
    
}

public class GetAllBookingsResponse
{
    
}

public class CreateBookingHandler : IRequestHandler<GetAllBookingsRequest,GetAllBookingsResponse>
{
    public Task<GetAllBookingsResponse> Handle(GetAllBookingsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}