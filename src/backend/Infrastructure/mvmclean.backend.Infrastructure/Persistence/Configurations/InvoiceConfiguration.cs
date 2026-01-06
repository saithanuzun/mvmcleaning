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

        builder.Property(i => i.InvoiceNumber)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(i => i.BookingId)
            .IsRequired();
        builder.Property(i => i.CustomerId)
            .IsRequired();
        builder.Property(i => i.IssueDate)
            .IsRequired();
        builder.Property(i => i.DueDate)
            .IsRequired();
        builder.Property(i => i.Status)
            .IsRequired();

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
                
            li.OwnsOne(i => i.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice_Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                money.Property(m => m.Currency)
                    .HasColumnName("UnitPrice_Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        // PaymentTerms value object
        builder.OwnsOne(i => i.PaymentTerms, terms =>
        {
            terms.Property(t => t.DaysToPay)
                .HasColumnName("PaymentTerms_DaysToPay")
                .IsRequired();
            terms.Property(t => t.EarlyPaymentDiscountPercent)
                .HasColumnName("PaymentTerms_EarlyPaymentDiscountPercent")
                .HasColumnType("decimal(5,2)")
                .IsRequired();
            terms.Property(t => t.EarlyPaymentDiscountDays)
                .HasColumnName("PaymentTerms_EarlyPaymentDiscountDays")
                .IsRequired();
        });

        // DiscountAmount value object
        builder.OwnsOne(i => i.DiscountAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DiscountAmount_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("DiscountAmount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Subtotal value object
        builder.OwnsOne(i => i.Subtotal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Subtotal_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("Subtotal_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // TotalAmount value object
        builder.OwnsOne(i => i.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("TotalAmount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Relationships
        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(i => i.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();
        builder.HasIndex(i => i.BookingId);
        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.DueDate);






    }
}