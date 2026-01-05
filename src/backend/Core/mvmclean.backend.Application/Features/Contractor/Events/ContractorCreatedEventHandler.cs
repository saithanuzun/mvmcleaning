using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor.Events;

namespace mvmclean.backend.Application.Features.Contractor.Events;

public class BookingConfirmedEventHandler : INotificationHandler<ContractorCreatedEvent>
{
    public Task Handle(ContractorCreatedEvent notification, CancellationToken cancellationToken)
    {
        //todo
        
        return Task.CompletedTask;
    }
}