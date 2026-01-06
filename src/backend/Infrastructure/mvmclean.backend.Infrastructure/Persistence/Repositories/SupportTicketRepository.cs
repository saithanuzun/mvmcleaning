using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.SupportTicket;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class SupportTicketRepository : GenericRepository<SupportTicket>, ISupportTicketRepository
{
    public SupportTicketRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}
