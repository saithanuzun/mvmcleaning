using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : EntityConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);

        builder.OwnsOne(i => i.Amount);
        
    }
}