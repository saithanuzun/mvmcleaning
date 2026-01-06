using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Booking.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : EntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        

        // Address value object
        builder.OwnsOne(c => c.Address, address =>
        {
            address.OwnsOne(a => a.Postcode);
        });

        // Email value object (nullable)
        builder.OwnsOne(i => i.Email);
        
        // PhoneNumber value object
        builder.OwnsOne(i => i.PhoneNumber);
        
        // Messages collection
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Customer)
            .HasForeignKey(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
            
    }
}