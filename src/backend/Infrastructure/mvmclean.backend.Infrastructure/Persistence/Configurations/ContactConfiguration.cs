using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contact;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : EntityConfiguration<Contact>
{
    public override void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.OwnsMany(c => c.Messages, messages =>
        {
           
        });

    }
}
