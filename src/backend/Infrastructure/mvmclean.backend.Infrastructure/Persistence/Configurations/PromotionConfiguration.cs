using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Promotion;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PromotionConfiguration : EntityConfiguration<Promotion>
{
    public override void Configure(EntityTypeBuilder<Promotion> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.Description)
            .HasMaxLength(1000);
        builder.Property(p => p.DiscountType)
            .IsRequired();
        builder.Property(p => p.DiscountValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(p => p.ValidFrom)
            .IsRequired();
        builder.Property(p => p.ValidTo)
            .IsRequired();
        builder.Property(p => p.UsageLimit)
            .IsRequired();
        builder.Property(p => p.UsedCount)
            .IsRequired();
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // MinimumOrderAmount value object
        builder.OwnsOne(i => i.MinimumOrderAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MinimumOrderAmount_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("MinimumOrderAmount_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // ApplicablePostcodes collection
        builder.OwnsMany(i => i.ApplicablePostcodes, a =>
        {
            a.WithOwner().HasForeignKey("PromotionId");
            a.ToTable("PromotionApplicablePostcodes");
            a.Property<Guid>("Id").ValueGeneratedNever();
            a.HasKey("Id");
            
            a.Property(p => p.Value)
                .HasColumnName("Postcode_Value")
                .HasMaxLength(10)
                .IsRequired();
            a.Property(p => p.Area)
                .HasColumnName("Postcode_Area")
                .HasMaxLength(2);
            a.Property(p => p.District)
                .HasColumnName("Postcode_District")
                .HasMaxLength(4);
            a.Property(p => p.Sector)
                .HasColumnName("Postcode_Sector")
                .HasMaxLength(8);
        });
        
        // ApplicableServices collection (List<Guid>) with value comparer
        builder.Property(p => p.ApplicableServices)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .HasMaxLength(2000)
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
            
        // Indexes
        builder.HasIndex(p => p.Code)
            .IsUnique();
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.ValidFrom);
        builder.HasIndex(p => p.ValidTo);
    }
}