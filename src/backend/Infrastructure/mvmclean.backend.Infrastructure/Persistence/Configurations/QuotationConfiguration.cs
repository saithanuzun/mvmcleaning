using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Quotation;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class QuotationConfiguration : EntityConfiguration<Quotation>
{
    public override void Configure(EntityTypeBuilder<Quotation> builder)
    {
        throw new NotImplementedException();
    }
}