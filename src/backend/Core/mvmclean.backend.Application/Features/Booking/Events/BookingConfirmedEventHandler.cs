using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking.Events;

namespace mvmclean.backend.Application.Features.Booking.Events;

public class BookingConfirmedEventHandler : INotificationHandler<BookingConfirmedEvent>
{
    public async Task Handle(BookingConfirmedEvent notification, CancellationToken cancellationToken)
    {
        //todo
        // Send SMS
        // Send email and [post contractor unavailibity

        Console.WriteLine(
            $"Quotation created: {notification}, "
        );

        await Task.CompletedTask;
    }
}

