using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class PaymentCompletedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid PaymentId { get; }
    public decimal Amount { get; }

    public PaymentCompletedEvent(Guid bookingId, Guid paymentId, decimal amount): base()
    {
        BookingId = bookingId;
        PaymentId = paymentId;
        Amount = amount;
    }
}