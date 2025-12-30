using MediatR;

namespace mvmclean.backend.Domain.Core.Interfaces;

public interface IDomainEvent : INotification //Mediatr
{
    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Unique identifier for this event
    /// </summary>
    Guid EventId { get; }
}