using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Invoice;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : EntityConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);

        // LineItems collection
        builder.OwnsMany(o => o.LineItems, li =>
        {
                
            li.OwnsOne(i => i.UnitPrice);
        });

        // PaymentTerms value object
        builder.OwnsOne(i => i.PaymentTerms);

        // DiscountAmount value object
        builder.OwnsOne(i => i.DiscountAmount);
        
        // Subtotal value object
        builder.OwnsOne(i => i.Subtotal);
        
        // TotalAmount value object
        builder.OwnsOne(i => i.TotalAmount);
        builder.OwnsOne(i => i.Address, li=>li.OwnsOne(i=>i.Postcode));


        
        // Relationships
        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(i => i.BookingId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}