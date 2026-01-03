using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Invoice;
using mvmclean.backend.Domain.Aggregates.Quotation;
using mvmclean.backend.Domain.Aggregates.SeoPage;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.Aggregates.SupportTicket;
using mvmclean.backend.Domain.Aggregates.SupportTicket.Entities;
using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Infrastructure.Persistence;

public class MVMdbContext : DbContext
{
    public MVMdbContext(DbContextOptions<MVMdbContext> options)
        : base(options)
    {
    }

    public MVMdbContext() : base()
    {
        
    }

    public DbSet<Contractor> Contractors { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Quotation> Quotations { get; set; } = null!;
    public DbSet<SeoPage> SeoPages { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
    
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(MVMdbContext).Assembly);
    
        // Global query filters
        ApplyGlobalQueryFilters(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            var connStr =
                "USER ID=postgres ; Password=password123;Server=localhost;Port=5432;Database=MVMTest;Pooling=true";
            optionsBuilder.UseNpgsql(connStr, opt => { opt.EnableRetryOnFailure(); });
        }
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>();
    }



    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var notExpression = Expression.Not(isDeletedProperty);
                var lambda = Expression.Lambda(notExpression, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
    
}