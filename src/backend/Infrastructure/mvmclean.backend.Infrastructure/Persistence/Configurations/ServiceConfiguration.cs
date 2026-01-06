using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : EntityConfiguration<Service>
{
    public override void Configure(EntityTypeBuilder<Service> builder)
    {
        base.Configure(builder);
        
        builder.Property(s => s.Name)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(s => s.Description)
            .HasMaxLength(2000);
        builder.Property(s => s.Shortcut)
            .HasMaxLength(50);
        builder.Property(s => s.EstimatedDuration)
            .IsRequired();
        
        // BasePrice value object
        builder.OwnsOne(i => i.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BasePrice_Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("BasePrice_Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        
        // Category relationship
        builder.HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // PostcodePricings collection
        builder.HasMany(s => s.PostcodePricings)
            .WithOne(p => p.Service)
            .HasForeignKey(p => p.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(s => s.Shortcut);
        builder.HasIndex(s => s.CategoryId);
    }
}