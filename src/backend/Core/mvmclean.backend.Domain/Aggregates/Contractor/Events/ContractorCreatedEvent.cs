using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Events;

public class ContractorCreatedEvent : DomainEvent
{
    public ContractorCreatedEvent(string contractorId, string contractorFullName, string contractorEmail)
    {
        ContractorId = contractorId;
        ContractorFullName = contractorFullName;
        ContractorEmail = contractorEmail;
    }

    public string ContractorId { get; set; }
    public string ContractorFullName { get; set; }
    public string ContractorEmail { get; set; }
}