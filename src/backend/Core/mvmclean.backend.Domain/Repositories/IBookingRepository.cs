using mvmclean.backend.Domain.AggregateRoot;

namespace mvmclean.backend.Domain.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
