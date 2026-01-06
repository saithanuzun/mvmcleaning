using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PostcodePricingConfiguration : EntityConfiguration<PostcodePricing>
{
    public override void Configure(EntityTypeBuilder<PostcodePricing> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.ServiceId)
            .IsRequired();
        builder.Property(p => p.Multiplier)
            .HasColumnType("decimal(5,2)")
            .IsRequired();
        builder.Property(p => p.FixedAdjustment)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Postcode value object
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
        
        builder.HasOne(i => i.Service)
            .WithMany(s => s.PostcodePricings)
            .HasForeignKey(i => i.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(p => p.ServiceId);
    }
}