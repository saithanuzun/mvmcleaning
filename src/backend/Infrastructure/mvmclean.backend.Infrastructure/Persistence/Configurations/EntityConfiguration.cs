using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();
        
        builder.Property(i => i.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever();
            
        builder.Property(i => i.CreatedBy)
            .HasMaxLength(256);
            
        builder.Property(i => i.UpdatedAt)
            .ValueGeneratedNever();
            
        builder.Property(i => i.UpdatedBy)
            .HasMaxLength(256);
            
        // Soft delete configuration
        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(i => i.DeletedAt);
            
        builder.Property(i => i.DeletedBy)
            .HasMaxLength(256);
    }
}