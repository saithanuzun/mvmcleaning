using mvmclean.backend.Domain.Entities;

namespace mvmclean.backend.Domain.Repositories;

public interface IPackageRepository : IRepository<Package>
{
    Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
