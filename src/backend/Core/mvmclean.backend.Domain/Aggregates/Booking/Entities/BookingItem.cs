using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class BookingItem : Entity
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; }
    public Money BasePrice { get; set; } // Original price without postcode adjustment
    public Money AdjustedPrice { get; set; } // Price after postcode adjustment
    public int Quantity { get; set; }
}
