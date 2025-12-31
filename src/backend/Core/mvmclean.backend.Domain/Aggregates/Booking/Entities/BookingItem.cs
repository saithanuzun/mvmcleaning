using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class BookingItem : Entity
{
    public Guid ServiceId { get; set; }
    public Money AdjustedPrice { get; set; } // Price after postcode adjustment
    public int Quantity { get; set; }
}
