using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorConfiguration : EntityConfiguration<Contractor>
{
    public override void Configure(EntityTypeBuilder<Contractor> builder)
    {
        base.Configure(builder);


        builder.OwnsOne(i => i.Email);
        
        builder.OwnsOne(i => i.PhoneNumber);
        
        builder.OwnsMany(c => c.UnavailableSlots, nav =>
        {
            nav.Property(ts => ts.StartTime)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            nav.Property(ts => ts.EndTime)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
        
        builder.HasMany(c => c.CoverageAreas)
            .WithOne(ca => ca.Contractor)
            .HasForeignKey(x => x.ContractorId);

        // WorkingHours collection
        builder.HasMany(c => c.WorkingHours)
            .WithOne(i=>i.Contractor)
            .HasForeignKey(x => x.ContractorId);
        
        builder.HasMany(c => c.Services)
            .WithOne(i => i.Contractor)
            .HasForeignKey(x => x.ContractorId);
        
        // Reviews collection
        builder.HasMany<Review>()
            .WithOne(r => r.Contractor)
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
  
    }
}