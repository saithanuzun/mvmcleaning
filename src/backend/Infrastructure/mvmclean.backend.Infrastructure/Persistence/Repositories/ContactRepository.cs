using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Contact;
using mvmclean.backend.Infrastructure.Persistence;
using mvmclean.backend.Infrastructure.Persistence.Repositories;

namespace mvmclean.backend.Infrastructure.Repositories;

public class ContactRepository : GenericRepository<Contact>, IContactRepository
{
    public ContactRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}
