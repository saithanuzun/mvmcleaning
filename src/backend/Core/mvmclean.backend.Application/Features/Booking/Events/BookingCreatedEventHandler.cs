using MediatR;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Booking.Events;

namespace mvmclean.backend.Application.Features.Booking.Events;

public class BookingCreatedEventHandler: INotificationHandler<BookingCreatedEvent>
{
    private readonly IMailingService _mailingService;

    public BookingCreatedEventHandler(IMailingService mailingService)
    {
        _mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Send booking created notification with event data
            await _mailingService.SendBookingCreatedNotificationAsync(
                recipientEmail: "saithan.uzun@gmail.com", // TODO: get email form somwehere else
                bookingId: notification.BookingId,
                postcode: notification.Postcode.Value,
                telephoneNumber: notification.PhoneNumber.Value
            );
            
            // todo get contarctor repostiry find cotnractor by coverae area by postcode and send this email

            Console.WriteLine(
                $"Booking created event handled: BookingId: {notification.BookingId}, " +
                $"Postcode: {notification.Postcode.Value}, " +
                $"Phone: {notification.PhoneNumber.Value}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling BookingCreatedEvent: {ex.Message}");
            throw;
        }
    }
}