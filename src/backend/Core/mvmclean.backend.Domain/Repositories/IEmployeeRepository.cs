using mvmclean.backend.Domain.AggregateRoot;

namespace mvmclean.backend.Domain.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
