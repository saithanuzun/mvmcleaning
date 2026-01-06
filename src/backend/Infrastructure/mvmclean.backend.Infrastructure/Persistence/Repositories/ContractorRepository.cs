using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class ContractorRepository : GenericRepository<Contractor>, IContractorRepository
{
    private readonly MVMdbContext _dbContext;

    public ContractorRepository(MVMdbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Contractor> GetByIdAsync(Guid id, bool noTracking = true, params System.Linq.Expressions.Expression<Func<Contractor, object>>[] includes)
    {
        var query = _dbContext.Set<Contractor>()
            .Include(c => c.CoverageAreas)
            .Include(c => c.WorkingHours)
            .Include(c => c.UnavailableSlots)
            .Include(c => c.Reviews)
            .Where(c => c.Id == id);

        if (noTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}