using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Events;

public class BookingConfirmedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public Guid? ContractorId { get; }
    public TimeSlot TimeSlot { get; set; }
    public Money Total { get; set; }
    
    public BookingConfirmedEvent(Guid bookingId, Guid? contractorId, Money total, TimeSlot timeSlot): base()
    {
        Total = total;
        TimeSlot = timeSlot;
        BookingId = bookingId;
        ContractorId = contractorId;
    }
}
