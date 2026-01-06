using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class BookingItem : Entity
{
    public string ServiceName { get; set; }
    public Guid ServiceId { get; set; }
    public Money UnitAdjustedPrice { get; set; } 
    public int Quantity { get; set; }

    public BookingItem(){}
    
    

    public void UpdateQuantity(int quantity)
    {
        Quantity =  quantity;
    }

}
