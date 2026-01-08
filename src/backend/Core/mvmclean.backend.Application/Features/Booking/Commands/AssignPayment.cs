using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Booking.Commands;

public class AssignPaymentRequest : IRequest<AssignPaymentResponse>
{
    public string BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; }
    public string PaymentLink { get; set; }
    
}

public class AssignPaymentResponse
{
    public string BookingId { get; set; }
}

public class AssignPaymentHandler : IRequestHandler<AssignPaymentRequest, AssignPaymentResponse>
{
    private IBookingRepository _bookingRepository;

    public AssignPaymentHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<AssignPaymentResponse> Handle(AssignPaymentRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(Guid.Parse(request.BookingId));
        var paymentType = (PaymentType)Enum.Parse(typeof(PaymentType), request.PaymentType);
        
        var payment = Payment.Create(Guid.Parse(request.BookingId), Money.Create(request.Amount), paymentType, request.PaymentLink);
            
        booking.AssignPayment(payment);

        await _bookingRepository.SaveChangesAsync();

        return new AssignPaymentResponse
        {
            BookingId = booking.Id.ToString(),
        };
    }

}

