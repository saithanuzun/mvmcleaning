using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Entities;

public class ContractorCoverage : Entity
{
    public Guid ContractorId { get; private set; }
    public Postcode Postcode { get; private set; }
    public bool IsActive { get; private set; }

    private ContractorCoverage() { }

    public static ContractorCoverage Create(Guid employeeId, Postcode postcode)
    {
        return new ContractorCoverage
        {
            ContractorId = employeeId,
            Postcode = postcode,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
