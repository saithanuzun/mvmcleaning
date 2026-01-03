using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class SupportTicketConfiguration : EntityConfiguration<SupportTicket>
{
    public override void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        base.Configure(builder);
        
        
        
    }
    
}