using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class ServiceRepository : GenericRepository<Service>, IServiceRepository
{
    public ServiceRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}