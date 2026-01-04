using MediatR;

namespace mvmclean.backend.Domain.Core.Interfaces;

public interface IDomainEvent : INotification //Mediatr
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}