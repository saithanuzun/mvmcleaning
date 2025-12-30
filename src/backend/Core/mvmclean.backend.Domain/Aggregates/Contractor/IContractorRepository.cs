using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Aggregates.Contractor;

public interface IContractorRepository : IRepository
{
    Task AddAsync(string? firstName, string? lastName, string? phoneNumber, string? email);
    Task<Contractor?> GetByIdAsync(Guid contractorId);
}