using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class ContractorConfiguration : EntityConfiguration<Contractor>
{
    public override void Configure(EntityTypeBuilder<Contractor> builder)
    {
        base.Configure(builder);


        builder.OwnsOne(i => i.Email);
        
        builder.OwnsOne(i => i.PhoneNumber);
        
        builder.OwnsMany(c => c.UnavailableSlots);
        
        builder.HasMany(c => c.CoverageAreas)
            .WithOne(ca => ca.Contractor)
            .HasForeignKey(x => x.ContractorId);

        // WorkingHours collection
        builder.OwnsMany(c => c.WorkingHours, a =>
        {
            a.WithOwner().HasForeignKey("ContractorId");
            a.ToTable("ContractorWorkingHours");
            a.Property<Guid>("Id").ValueGeneratedNever();
            a.HasKey("Id");
            
            a.Property(w => w.DayOfWeek)
                .IsRequired();
            a.Property(w => w.IsWorkingDay)
                .IsRequired();
            a.Property(w => w.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();
            a.Property(w => w.EndTime)
                .HasColumnName("EndTime")
                .IsRequired();
        });
        
        // Services collection (List<ServiceItem>)
        builder.OwnsMany(c => c.Services, a =>
        {
            a.WithOwner().HasForeignKey("ContractorId");
            a.ToTable("ContractorServices");
            a.Property<Guid>("Id").ValueGeneratedNever();
            a.HasKey("Id");
            
            a.Property(s => s.ServiceId)
                .IsRequired();
            a.Property(s => s.ServiceName)
                .HasMaxLength(255)
                .IsRequired();
            a.Property(s => s.Category)
                .HasMaxLength(100);
            a.Property(s => s.Description)
                .HasMaxLength(500);
        });
        
        // Reviews collection
        builder.HasMany<Review>()
            .WithOne(r => r.Contractor)
            .HasForeignKey(r => r.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
  
    }
}