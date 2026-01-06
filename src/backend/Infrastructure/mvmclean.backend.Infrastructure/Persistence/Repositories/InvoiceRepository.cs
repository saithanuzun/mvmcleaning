using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Invoice;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}
