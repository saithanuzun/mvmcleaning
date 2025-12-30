using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Aggregates.Booking;

public interface IBookingRepository : IRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByIdAsync(Guid bookingId);
}
