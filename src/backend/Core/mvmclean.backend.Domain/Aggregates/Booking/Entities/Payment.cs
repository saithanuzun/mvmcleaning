using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class Payment : Entity
{
    public Guid BookingId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }

    public PaymentType PaymentType { get; set; }
    
    public string? TransactionId { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    public static Payment Create(Guid bookingId, Money amount, PaymentType paymentType)
    {
        return new Payment
        {
            BookingId = bookingId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            PaymentType = paymentType,
        };
    }

    public void MarkAsAuthorized(string transactionId)
    {
        Status = PaymentStatus.Authorized;
        TransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCaptured()
    {
        Status = PaymentStatus.Captured;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}
