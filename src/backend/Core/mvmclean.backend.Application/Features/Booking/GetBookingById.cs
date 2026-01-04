using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Application.Features.Booking;

public class GetBookingByIdRequest : IRequest<GetBookingByIdResponse>
{
    public string Id { get; set; }
}

public class GetBookingByIdResponse
{
    
}

public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdRequest,GetBookingByIdResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingByIdHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<GetBookingByIdResponse> Handle(GetBookingByIdRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(Guid.Parse((request.Id)));
        
        return new GetBookingByIdResponse
        {
            
        };
    }
}