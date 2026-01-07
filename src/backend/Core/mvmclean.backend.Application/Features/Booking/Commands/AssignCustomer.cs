using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class AssignCustomerRequest :  IRequest<AssignCustomerResponse>
{
    public string BookingId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string AdditionalInfo { get; set; }
}

public class AssignCustomerResponse
{
    public string BookingId { get; set; }
    public string CustomerId { get; set; }
}


public class AssignCustomerHandler : IRequestHandler<AssignCustomerRequest, AssignCustomerResponse>
{
    private IBookingRepository  _bookingRepository;

    public AssignCustomerHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    public async Task<AssignCustomerResponse> Handle(AssignCustomerRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(Guid.Parse(request.BookingId));
        
        booking.AssignCustomer(Email.Create(request.Email),   
                request.FirstName, 
                request.LastName, 
                request.Street,
                request.City,
                request.AdditionalInfo);
        
        return new AssignCustomerResponse
        {
            BookingId = booking.Id.ToString(),
            CustomerId = booking.CustomerId.ToString()
        };
    }
}