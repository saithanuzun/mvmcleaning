using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class BookingConfiguration: EntityConfiguration<Booking>
{
    public override void Configure(EntityTypeBuilder<Booking> builder)
    {
        throw new NotImplementedException();
    }
}