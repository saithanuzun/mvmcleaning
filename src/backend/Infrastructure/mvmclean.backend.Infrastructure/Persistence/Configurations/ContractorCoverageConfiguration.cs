using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorCoverageConfiguration : EntityConfiguration<ContractorCoverage>
{
    public override void Configure(EntityTypeBuilder<ContractorCoverage> builder)
    {
        base.Configure(builder);
        
        builder.OwnsOne(i => i.Postcode);

        builder.HasOne(c => c.Contractor)
            .WithMany()
            .HasForeignKey(c => c.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}