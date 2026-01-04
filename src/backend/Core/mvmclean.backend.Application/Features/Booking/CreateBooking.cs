using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking;



public class CreateBookingRequest : IRequest<CreateBookingResponse>
{
    public string PhoneNumber { get; set; }
    public string Postcode { get; set; }
}

public class CreateBookingResponse
{
    public string BookingId { get; set; }
}

public class CreateBookingHandler : IRequestHandler<CreateBookingRequest,CreateBookingResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<CreateBookingResponse> Handle(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = Domain.Aggregates.Booking.Booking.Create(Postcode.Create(request.Postcode), PhoneNumber.Create(request.PhoneNumber));

        return new CreateBookingResponse
        {
            BookingId = booking.Id.ToString()
        };


    }
}