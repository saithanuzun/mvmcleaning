using Microsoft.Extensions.Logging;
using mvmclean.backend.Application.Services;
using mvmclean.backend.Infrastructure.MailingService;
using Resend;

namespace mvmclean.backend.Infrastructure.Services;

/// <summary>
/// Mailing service implementation using email templates
/// Handles all transactional emails for the booking system
/// </summary>
public class MailingService : IMailingService
{
    private readonly ILogger<MailingService> _logger;
    private readonly IResend _resendClient;

    public MailingService(ILogger<MailingService> logger, IResend resendClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resendClient = resendClient ?? throw new ArgumentNullException(nameof(resendClient));
    }

    /// <summary>
    /// Sends booking confirmation email after payment is completed
    /// </summary>
    public async Task SendBookingConfirmationAsync(
        string recipientEmail,
        Guid bookingId,
        string customerName,
        string address,
        List<string> services,
        decimal totalAmount,
        DateTime bookingDate,
        string? invoiceHtml = null)
    {
        try
        {
            // Generate email template
            var template = new BookingConfirmedEmailTemplate(
                bookingId.ToString(),
                customerName,
                address,
                services,
                totalAmount,
                bookingDate,
                invoiceHtml
            );

            await SendEmailAsync(recipientEmail, template.Subject, template.HtmlBody, template.PlainTextBody);
            
            _logger.LogInformation(
                "Booking confirmation email sent to {Email} for booking {BookingId}",
                recipientEmail, bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send booking confirmation email to {Email} for booking {BookingId}",
                recipientEmail, bookingId);
            throw;
        }
    }

    /// <summary>
    /// Sends booking created notification (before payment)
    /// Shows postcode and telephone number
    /// </summary>
    public async Task SendBookingCreatedNotificationAsync(
        string recipientEmail,
        Guid bookingId,
        string postcode,
        string telephoneNumber)
    {
        try
        {
            var template = new BookingCreatedEmailTemplate(
                bookingId.ToString(),
                postcode,
                telephoneNumber
            );

            await SendEmailAsync(recipientEmail, template.Subject, template.HtmlBody, template.PlainTextBody);
            
            _logger.LogInformation(
                "Booking created email sent to {Email} for booking {BookingId}",
                recipientEmail, bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send booking created email to {Email} for booking {BookingId}",
                recipientEmail, bookingId);
            throw;
        }
    }

    public Task SendContractorBookingNotificationAsync(
        string contractorEmail,
        string contractorName,
        Guid bookingId,
        DateTime bookingDate,
        string customerName,
        string customerAddress)
    {
        // TODO: Implement contractor notification email template
        _logger.LogInformation(
            "Contractor notification queued for {Email} regarding booking {BookingId}",
            contractorEmail, bookingId);
        return Task.CompletedTask;
    }

    public Task SendPaymentReceiptAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        decimal amount,
        string paymentMethod,
        DateTime paymentDate)
    {
        // TODO: Implement payment receipt email template
        _logger.LogInformation(
            "Payment receipt queued for {Email} regarding booking {BookingId}",
            recipientEmail, bookingId);
        return Task.CompletedTask;
    }

    public Task SendBookingCancellationAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        string reason)
    {
        // TODO: Implement booking cancellation email template
        _logger.LogInformation(
            "Cancellation email queued for {Email} regarding booking {BookingId}",
            recipientEmail, bookingId);
        return Task.CompletedTask;
    }

    public Task SendBookingReminderAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        List<string> services,
        string address)
    {
        // TODO: Implement booking reminder email template
        _logger.LogInformation(
            "Reminder email queued for {Email} regarding booking {BookingId}",
            recipientEmail, bookingId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Generic email sending method using Resend email provider
    /// </summary>
    public async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string? textBody = null)
    {
        try
        {
            var emailRequest = new EmailMessage
            {
                From = "noreply@mvmcleaning.com", // Replace with your verified sender email
                To = new[] { recipientEmail },
                Subject = subject,
                HtmlBody = htmlBody,
                TextBody = textBody ?? StripHtmlTags(htmlBody)
            };

            var response = await _resendClient.EmailSendAsync(emailRequest);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred while sending email to {To} with subject {Subject}",
                recipientEmail, subject);
            throw;
        }
    }

   
    private static string StripHtmlTags(string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        var regex = new System.Text.RegularExpressions.Regex("<[^>]+>");
        return regex.Replace(html, "").Replace("&nbsp;", " ");
    }
}
