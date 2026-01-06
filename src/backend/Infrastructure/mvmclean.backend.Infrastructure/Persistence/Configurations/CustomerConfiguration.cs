using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : EntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.FirstName)
            .HasMaxLength(200);
        builder.Property(c => c.LastName)
            .HasMaxLength(200);

        // Address value object
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(500)
                .IsRequired();
            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(a => a.AdditionalInfo)
                .HasColumnName("Address_AdditionalInfo")
                .HasMaxLength(1000);
            address.Property(a => a.Latitude)
                .HasColumnName("Address_Latitude")
                .HasColumnType("double precision");
            address.Property(a => a.Longitude)
                .HasColumnName("Address_Longitude")
                .HasColumnType("double precision");
                
            address.OwnsOne(a => a.Postcode, postcode =>
            {
                postcode.Property(p => p.Value)
                    .HasColumnName("Address_Postcode_Value")
                    .HasMaxLength(10)
                    .IsRequired();
                postcode.Property(p => p.Area)
                    .HasColumnName("Address_Postcode_Area")
                    .HasMaxLength(2);
                postcode.Property(p => p.District)
                    .HasColumnName("Address_Postcode_District")
                    .HasMaxLength(4);
                postcode.Property(p => p.Sector)
                    .HasColumnName("Address_Postcode_Sector")
                    .HasMaxLength(8);
            });
        });

        // Email value object (nullable)
        builder.OwnsOne(i => i.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email_Value")
                .HasMaxLength(256);
            email.Property(e => e.NormalizedValue)
                .HasColumnName("Email_NormalizedValue")
                .HasMaxLength(256);
        });
        
        // PhoneNumber value object
        builder.OwnsOne(i => i.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber_Value")
                .HasMaxLength(20)
                .IsRequired();
        });
        
        // Messages collection
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Customer)
            .HasForeignKey(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
            
    }
}