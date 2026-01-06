using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.SupportTicket.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class TicketMessageConfiguration : EntityConfiguration<TicketMessage>
{
    public override void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        base.Configure(builder);
        
        builder.HasOne(m => m.SupportTicket)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.SupportTicketId);
        
    }
}
