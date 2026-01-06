using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class BookingConfiguration: EntityConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        base.Configure(builder);
        
        // TotalPrice value object
        builder.OwnsOne(i => i.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalPrice_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("TotalPrice_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        builder.OwnsOne(i => i.ServiceAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ServiceAddress_Street")
                .HasMaxLength(500)
                .IsRequired();
            address.Property(a => a.City)
                .HasColumnName("ServiceAddress_City")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(a => a.AdditionalInfo)
                .HasColumnName("ServiceAddress_AdditionalInfo")
                .HasMaxLength(1000);
            address.Property(a => a.Latitude)
                .HasColumnName("ServiceAddress_Latitude")
                .HasColumnType("double precision");
            address.Property(a => a.Longitude)
                .HasColumnName("ServiceAddress_Longitude")
                .HasColumnType("double precision");
                
            address.OwnsOne(a => a.Postcode, postcode =>
            {
                postcode.Property(p => p.Value)
                    .HasColumnName("ServiceAddress_Postcode_Value")
                    .HasMaxLength(10)
                    .IsRequired();
                postcode.Property(p => p.Area)
                    .HasColumnName("ServiceAddress_Postcode_Area")
                    .HasMaxLength(2);
                postcode.Property(p => p.District)
                    .HasColumnName("ServiceAddress_Postcode_District")
                    .HasMaxLength(4);
                postcode.Property(p => p.Sector)
                    .HasColumnName("ServiceAddress_Postcode_Sector")
                    .HasMaxLength(8);
            });
        });

        builder.OwnsOne(i => i.ScheduledSlot, slot =>
        {
            slot.Property(s => s.StartTime)
                .HasColumnName("ScheduledSlot_StartTime")
                .IsRequired();
            slot.Property(s => s.EndTime)
                .HasColumnName("ScheduledSlot_EndTime")
                .IsRequired();
        });
        
        builder.OwnsMany(o => o.ServiceItems, li =>
        {
            li.WithOwner().HasForeignKey("BookingId");
            li.ToTable("BookingServiceItems");
            li.Property<Guid>("Id").ValueGeneratedNever();
            li.HasKey("Id");
            
            li.Property(i => i.ServiceName)
                .HasMaxLength(500)
                .IsRequired();
            li.Property(i => i.ServiceId)
                .IsRequired();
            li.Property(i => i.Quantity)
                .IsRequired();
                
            li.OwnsOne(i => i.UnitAdjustedPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitAdjustedPrice_Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                money.Property(m => m.Currency)
                    .HasColumnName("UnitAdjustedPrice_Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.OwnsOne(i => i.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber_Value")
                .HasMaxLength(20)
                .IsRequired();
        });
        
        builder.OwnsOne(i => i.Postcode, postcode =>
        {
            postcode.Property(p => p.Value)
                .HasColumnName("Postcode_Value")
                .HasMaxLength(10)
                .IsRequired();
            postcode.Property(p => p.Area)
                .HasColumnName("Postcode_Area")
                .HasMaxLength(2);
            postcode.Property(p => p.District)
                .HasColumnName("Postcode_District")
                .HasMaxLength(4);
            postcode.Property(p => p.Sector)
                .HasColumnName("Postcode_Sector")
                .HasMaxLength(8);
        });
        
        // Payment relationship
        builder.HasOne(b => b.Payment)
            .WithOne()
            .HasForeignKey<Booking>(b => b.PaymentId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // Contractor relationship
        builder.HasOne<Contractor>()
            .WithMany()
            .HasForeignKey(b => b.ContractorId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // Indexes
        builder.HasIndex(b => b.ContractorId);
        builder.HasIndex(b => b.CustomerId);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.CreationStatus);
    }
}