using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorConfiguration : EntityConfiguration<Contractor>
{
    public override void Configure(EntityTypeBuilder<Contractor> builder)
    {
        throw new NotImplementedException();
    }
}