using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Service.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PostcodePricingConfiguration : EntityConfiguration<PostcodePricing>
{
    public override void Configure(EntityTypeBuilder<PostcodePricing> builder)
    {
        base.Configure(builder);


        builder.OwnsOne(i => i.Postcode);
        
        builder.HasOne(i => i.Service)
            .WithMany(s => s.PostcodePricings)
            .HasForeignKey(i => i.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}