using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking.Events;

namespace mvmclean.backend.Application.Features.Booking.Events;

public class BookingConfirmedEventHandler : INotificationHandler<BookingConfirmedEvent>
{
    public Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

