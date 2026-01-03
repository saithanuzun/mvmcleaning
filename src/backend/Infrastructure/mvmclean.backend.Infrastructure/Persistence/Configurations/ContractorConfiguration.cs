using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorConfiguration : EntityConfiguration<Contractor>
{
    public override void Configure(EntityTypeBuilder<Contractor> builder)
    {
        base.Configure(builder);
        
        builder.OwnsMany(o => o.UnavailableSlots, li =>
        {
            
        });
        
        builder.OwnsOne(i => i.Email);
        
        builder.OwnsOne(i => i.PhoneNumber);
        
        builder.OwnsMany(i => i.CoverageAreas, li=>li.OwnsOne(i=>i.Postcode));

            
    }
}