using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Promotion;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
{
    public PromotionRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}
