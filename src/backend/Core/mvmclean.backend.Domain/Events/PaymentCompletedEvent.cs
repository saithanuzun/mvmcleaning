using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Events;

public class PaymentCompletedEvent : IDomainEvent
{
    public Guid BookingId { get; }
    public Guid PaymentId { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    public PaymentCompletedEvent(Guid bookingId, Guid paymentId, decimal amount)
    {
        BookingId = bookingId;
        PaymentId = paymentId;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
}