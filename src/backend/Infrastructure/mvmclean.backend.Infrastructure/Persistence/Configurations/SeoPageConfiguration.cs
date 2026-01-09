using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class SeoPageConfiguration : EntityConfiguration<SeoPage>
{
    public override void Configure(EntityTypeBuilder<SeoPage> builder)
    {
        base.Configure(builder);
        
        builder.OwnsMany(s => s.Areas, navBuilder =>
        {
            navBuilder.ToTable("SeoPage_Areas");
            navBuilder.HasKey("Id");
            navBuilder.Property(a => a.Name).IsRequired();
            navBuilder.Property(a => a.Slug).IsRequired();
            navBuilder.WithOwner().HasForeignKey("SeoPageId");
        });

        builder.OwnsMany(s => s.Services, navBuilder =>
        {
            navBuilder.ToTable("SeoPage_Services");
            navBuilder.HasKey("Id");
            navBuilder.Property(st => st.Name).IsRequired();
            navBuilder.Property(st => st.Slug).IsRequired();
            navBuilder.WithOwner().HasForeignKey("SeoPageId");
        });

        builder.OwnsMany(s => s.Keywords, navBuilder =>
        {
            navBuilder.ToTable("SeoPage_Keywords");
            navBuilder.HasKey("Id");
            navBuilder.Property(k => k.Keyword).IsRequired();
            navBuilder.Property(k => k.Category).IsRequired();
            navBuilder.WithOwner().HasForeignKey("SeoPageId");
        });
    }
}
