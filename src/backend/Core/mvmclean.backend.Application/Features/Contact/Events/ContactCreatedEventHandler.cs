using MediatR;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Domain.Aggregates.Contact.Events;

namespace mvmclean.backend.Application.Features.Contact.Events;

public class ContactCreatedEventHandler : INotificationHandler<ContactCreatedEvent>
{
    private readonly IMailingService _mailingService;
    private const string NotifyEmail = "saithan.uzun@gmail.com";

    public ContactCreatedEventHandler(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    public async Task Handle(ContactCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var emailSubject = $"New Contact Form Submission: {notification.Subject}";
            var emailBody = $@"
                <h2>New Contact Form Submission</h2>
                <p><strong>Contact ID:</strong> {notification.ContactId}</p>
                <p><strong>Name:</strong> {notification.FullName ?? "Not provided"}</p>
                <p><strong>Email:</strong> {notification.Email}</p>
                <p><strong>Phone:</strong> {notification.PhoneNumber ?? "Not provided"}</p>
                <p><strong>Subject:</strong> {notification.Subject}</p>
                <p><strong>Message:</strong></p>
                <p>{notification.Message}</p>
                <p><strong>Submitted At:</strong> {notification.CreatedAt:yyyy-MM-dd HH:mm:ss}</p>
                <hr>
                <p><a href='#'>View and respond to this contact in admin dashboard</a></p>
            ";

            await _mailingService.SendEmailAsync(NotifyEmail, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log but don't throw - prevent email failures from breaking domain operations
            Console.WriteLine($"Error sending contact notification email: {ex.Message}");
        }
    }
}
