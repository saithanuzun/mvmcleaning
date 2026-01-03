using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Quotation;
using mvmclean.backend.Infrastructure.Persistence;
using mvmclean.backend.Infrastructure.Persistence.Repositories;

namespace mvmclean.backend.Infrastructure;

public static class Registrations
{
    public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection serviceCollection)
    {
        const string connStr = "USER ID=postgres ; Password=password123;Server=localhost;Port=5432;Database=MVMTest;Pooling=true";

        serviceCollection.AddDbContext<MVMdbContext>(conf =>
        {
            conf.UseNpgsql(connStr, opt => { opt.EnableRetryOnFailure(); });
        });
        

        serviceCollection.AddScoped<IBookingRepository, BookingRepository>();
        serviceCollection.AddScoped<IContractorRepository, ContractorRepository>();
        serviceCollection.AddScoped<IQuotationRepository, QuotationRepository>();
        
        return serviceCollection;
    }
}
