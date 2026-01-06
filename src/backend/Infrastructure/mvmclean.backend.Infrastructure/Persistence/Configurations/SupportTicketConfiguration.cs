using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class SupportTicketConfiguration : EntityConfiguration<SupportTicket>
{
    public override void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        base.Configure(builder);
        
        builder.Property(t => t.CustomerId)
            .IsRequired();
        builder.Property(t => t.Subject)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(t => t.Description)
            .HasMaxLength(5000)
            .IsRequired();
        builder.Property(t => t.Status)
            .IsRequired();
        
        // Messages collection (TicketMessage is an Entity, not ValueObject)
        builder.HasMany(t => t.Messages)
            .WithOne(m => m.SupportTicket)
            .HasForeignKey(m => m.SupportTicketId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // AssignedTo relationship
        builder.HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // Indexes
        builder.HasIndex(t => t.CustomerId);
        builder.HasIndex(t => t.BookingId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.AssignedToId);
    }
}