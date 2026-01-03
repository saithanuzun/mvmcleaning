using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : EntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        
        builder.OwnsOne(c=>c.Address);
        
        builder.OwnsOne(c=>c.Address, o=>o.OwnsOne(i=>i.Postcode));

        
        builder.OwnsOne(i => i.Email);
        builder.OwnsOne(i => i.PhoneNumber);

    }
}