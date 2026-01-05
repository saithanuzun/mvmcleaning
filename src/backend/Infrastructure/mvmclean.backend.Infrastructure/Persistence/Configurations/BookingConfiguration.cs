using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class BookingConfiguration: EntityConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        base.Configure(builder);
        
        builder.OwnsOne(i => i.TotalPrice);
        
        builder.OwnsOne(i => i.ServiceAddress, address =>
        {
            address.OwnsOne(a => a.Postcode, postcode =>
            {
            });
    
        });

        builder.OwnsOne(i => i.ScheduledSlot);
        
        builder.OwnsMany(o => o.ServiceItems, li =>
        {
            li.OwnsOne(i => i.UnitAdjustedPrice, money =>
            {
            });
        });

        builder.HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId);
        
        builder.OwnsOne(i => i.PhoneNumber);
        
        builder.OwnsOne(i => i.Postcode);


        


    }
}