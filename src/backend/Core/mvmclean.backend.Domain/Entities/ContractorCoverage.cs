using mvmclean.backend.Domain.Common;
using mvmclean.backend.Domain.ValueObjects;

namespace mvmclean.backend.Domain.Entities;

public class ContractorCoverage : Entity
{
    public Guid EmployeeId { get; private set; }
    public Postcode Postcode { get; private set; }
    public bool IsActive { get; private set; }

    private ContractorCoverage() { }

    public static ContractorCoverage Create(Guid employeeId, Postcode postcode)
    {
        return new ContractorCoverage
        {
            EmployeeId = employeeId,
            Postcode = postcode,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
