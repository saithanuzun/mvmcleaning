namespace mvmclean.backend.Domain.Repositories.Booking;

public interface IBookingRepository
{
    IQueryable<AggregateRoot.Booking> GetAll();
    IQueryable<AggregateRoot.Booking> GetById(Guid id);
    IQueryable<AggregateRoot.Booking> GetByContractorId(Guid contractorId);
    
    
    
}