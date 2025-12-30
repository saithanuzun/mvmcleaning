using MediatR;

namespace mvmclean.backend.Application.Features.Booking.Commands;



public class CreateBookingRequest : IRequest<CreateBookingResponse>
{
    
}

public class CreateBookingResponse
{
    
}

public class CreateBookingHandler : IRequestHandler<CreateBookingRequest,CreateBookingResponse>
{
    public Task<CreateBookingResponse> Handle(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}