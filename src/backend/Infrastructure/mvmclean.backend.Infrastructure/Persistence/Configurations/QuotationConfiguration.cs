using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Quotation;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class QuotationConfiguration : EntityConfiguration<Quotation>
{
    public override void Configure(EntityTypeBuilder<Quotation> builder)
    {
        base.Configure(builder);
        
        builder.OwnsMany(o => o.BasketItems, li =>
        {
            li.OwnsOne(i => i.Price, money =>
            {
            });
        });
        
        builder.OwnsOne(i => i.Cost);
        builder.OwnsOne(i => i.PhoneNumber);
        
        builder.OwnsOne(i => i.Postcode);

        

    }
}