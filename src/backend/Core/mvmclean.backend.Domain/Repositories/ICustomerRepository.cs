using mvmclean.backend.Domain.AggregateRoot;

namespace mvmclean.backend.Domain.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
