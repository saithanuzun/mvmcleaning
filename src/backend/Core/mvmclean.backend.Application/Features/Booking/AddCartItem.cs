using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking;

public class AddCartItemRequest : IRequest<AddCartItemResponse>
{
    public string BookingId { get; set; }
    public string Name { get; set; }
    public string ServiceItemId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
}

public class AddCartItemResponse
{
    public string BookingId { get; set; }
}

public class AddCartItemHandler : IRequestHandler<AddCartItemRequest, AddCartItemResponse>
{
    private readonly IBookingRepository  _bookingRepository;

    public AddCartItemHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<AddCartItemResponse> Handle(AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(Guid.Parse(request.BookingId));
        
        booking.AddServiceToCart(request.Name,Guid.Parse(request.ServiceItemId),Money.Create(request.Price),request.Quantity);
        
        if (booking is null)
            throw new Exception("Booking not found");

        return new AddCartItemResponse
        {
            BookingId = booking.Id.ToString()
        };
    }
}