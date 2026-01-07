using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class ServiceRepository : GenericRepository<Service>, IServiceRepository
{
    private readonly MVMdbContext _dbContext;

    public ServiceRepository(MVMdbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Service> GetByIdAsync(Guid id, bool noTracking = true, params System.Linq.Expressions.Expression<Func<Service, object>>[] includes)
    {
        var query = _dbContext.Services
            .Include(s => s.Category)
            .AsQueryable();

        if (noTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }
}