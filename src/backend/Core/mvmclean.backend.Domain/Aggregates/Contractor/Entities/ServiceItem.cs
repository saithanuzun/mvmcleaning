using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Entities;

public class ServiceItem : Entity
{
    public Guid ContractorId { get; set; }
    public Contractor Contractor { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string Category { get; set; }

    public string Description { get; set; }

}