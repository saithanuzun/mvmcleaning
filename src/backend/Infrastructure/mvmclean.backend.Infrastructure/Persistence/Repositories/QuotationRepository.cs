using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Quotation;
using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class QuotationRepository : GenericRepository<Quotation>, IQuotationRepository
{
    public QuotationRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}