using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Aggregates.Quotation;

public interface IQuotationRepository : IRepository
{
    Task AddAsync(string phoneNumber, string postcode);
    Task<Quotation?> GetByIdAsync(Guid quotationId); 
}