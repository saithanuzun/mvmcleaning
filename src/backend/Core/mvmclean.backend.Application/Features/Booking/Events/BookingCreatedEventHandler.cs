using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking.Events;

namespace mvmclean.backend.Application.Features.Booking.Events;

public class BookingCreatedEventHandler: INotificationHandler<BookingCreatedEvent>
{
    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        //todo
        // Send SMS
        // Send email

        Console.WriteLine(
            $"Quotation created: {notification.OccurredOn}, " +
            $"Postcode: {notification.EventId}, " +
            $"Phone: {notification.BookingId}"
        );

        await Task.CompletedTask;
    }
}