using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : EntityConfiguration<Service>
{
    public override void Configure(EntityTypeBuilder<Service> builder)
    {
        base.Configure(builder);
        
        builder.OwnsOne(i => i.BasePrice);
        
        
    }
}