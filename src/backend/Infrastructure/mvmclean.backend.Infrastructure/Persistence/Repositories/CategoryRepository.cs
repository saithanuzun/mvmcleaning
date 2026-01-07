using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.Aggregates.Service.Entities;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}