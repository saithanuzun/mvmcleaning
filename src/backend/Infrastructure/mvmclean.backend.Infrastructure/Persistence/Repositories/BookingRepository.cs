using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Domain.Aggregates.Booking;

namespace mvmclean.backend.Infrastructure.Persistence.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(MVMdbContext dbContext) : base(dbContext)
    {
    }
}