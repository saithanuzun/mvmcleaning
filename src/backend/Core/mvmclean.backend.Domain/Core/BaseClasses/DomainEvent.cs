using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Core.BaseClasses;

/// <summary>
/// Base class for domain events with common properties
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}