using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Promotion;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PromotionConfiguration : EntityConfiguration<Promotion>
{
    public override void Configure(EntityTypeBuilder<Promotion> builder)
    {
        base.Configure(builder);
        

        builder.OwnsOne(i => i.MinimumOrderAmount);

        // ApplicablePostcodes collection
        builder.OwnsMany(i => i.ApplicablePostcodes);
        

    }
}