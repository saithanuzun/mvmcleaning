using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorWorkingHoursConfiguration : EntityConfiguration<WorkingHours>
{
    public override void Configure(EntityTypeBuilder<WorkingHours> builder)
    {
        base.Configure(builder);

        // Configure the DayOfWeek property
        builder.Property(w => w.DayOfWeek)
            .IsRequired()
            .HasConversion<int>();

        // Configure the IsWorkingDay property
        builder.Property(w => w.IsWorkingDay)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure the StartTime property
        builder.Property(w => w.StartTime)
            .IsRequired()
            .HasConversion<TimeOnly>();

        // Configure the EndTime property
        builder.Property(w => w.EndTime)
            .IsRequired()
            .HasConversion<TimeOnly>();

        // Configure the foreign key relationship
        builder.HasOne(w => w.Contractor)
            .WithMany(c => c.WorkingHours)
            .HasForeignKey(w => w.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}