using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class SeoPageConfiguration : EntityConfiguration<SeoPage>
{
    public override void Configure(EntityTypeBuilder<SeoPage> builder)
    {
        base.Configure(builder);
        
        
        builder.HasMany(s => s.ContentBlocks)
            .WithOne(c => c.SeoPage)
            .HasForeignKey(c => c.SeoPageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.FAQs)
            .WithOne(f => f.SeoPage)
            .HasForeignKey(f => f.SeoPageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.Keywords)
            .WithOne()
            .HasForeignKey("SeoPageId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
