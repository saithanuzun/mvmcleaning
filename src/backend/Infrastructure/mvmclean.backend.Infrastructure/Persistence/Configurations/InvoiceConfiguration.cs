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
            li.WithOwner().HasForeignKey("InvoiceId");
            li.ToTable("InvoiceLineItems");
            li.Property<Guid>("Id").ValueGeneratedNever();
            li.HasKey("Id");
            
            li.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();
            li.Property(x => x.Quantity)
                .IsRequired();
                
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
        
        // Relationships
        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(i => i.BookingId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}