using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : EntityConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.BookingId)
            .IsRequired();
        builder.Property(p => p.PaymentLink)
            .HasMaxLength(1000);
        builder.Property(p => p.TransactionId)
            .HasMaxLength(200);
        builder.Property(p => p.FailureReason)
            .HasMaxLength(1000);
        builder.Property(p => p.Status)
            .IsRequired();
        builder.Property(p => p.PaymentType)
            .IsRequired();

        // Amount value object
        builder.OwnsOne(i => i.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("Amount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Booking relationship
        builder.HasOne<Booking>()
            .WithOne(b => b.Payment)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(p => p.BookingId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransactionId);
    }
}