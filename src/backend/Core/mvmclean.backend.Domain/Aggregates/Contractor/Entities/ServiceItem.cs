using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Entities;

public class ServiceItem : ValueObject
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string Category { get; set; }

    public string Description { get; set; }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}