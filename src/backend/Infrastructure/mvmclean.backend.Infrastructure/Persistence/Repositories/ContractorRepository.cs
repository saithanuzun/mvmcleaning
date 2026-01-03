using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class ContractorRepository : GenericRepository<Contractor>, IContractorRepository
{
    public ContractorRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}