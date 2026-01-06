using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class SeoPageRepository : GenericRepository<SeoPage>, ISeoPageRepository
{
    public SeoPageRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}
