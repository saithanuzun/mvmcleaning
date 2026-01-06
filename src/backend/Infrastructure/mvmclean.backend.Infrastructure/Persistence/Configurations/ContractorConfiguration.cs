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
        
        builder.OwnsMany(c => c.UnavailableSlots);
        
        builder.HasMany(c => c.CoverageAreas)
            .WithOne(ca => ca.Contractor)
            .HasForeignKey(x => x.ContractorId);

        // WorkingHours collection
        builder.HasMany(c => c.WorkingHours)
            .WithOne(i=>i.Contractor)
            .HasForeignKey(x => x.ContractorId);
        
        // Services collection (List<ServiceItem>)
        builder.OwnsMany(c => c.Services);
        
        // Reviews collection
        builder.HasMany<Review>()
            .WithOne(r => r.Contractor)
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
  
    }
}