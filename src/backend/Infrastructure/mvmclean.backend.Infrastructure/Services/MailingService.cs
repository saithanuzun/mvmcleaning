using mvmclean.backend.Application.Services;

namespace mvmclean.backend.Infrastructure.Services;

/// <summary>
/// Stub implementation of Mailing service - to be implemented with actual email provider (SendGrid, SMTP, etc.)
/// </summary>
public class MailingService : IMailingService
{
    public Task SendBookingConfirmationAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        List<string> services,
        decimal totalAmount)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Booking confirmation sent to {recipientEmail}");
        Console.WriteLine($"  Recipient: {recipientName}");
        Console.WriteLine($"  Booking ID: {bookingId}");
        Console.WriteLine($"  Date: {bookingDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Services: {string.Join(", ", services)}");
        Console.WriteLine($"  Total: £{totalAmount:F2}");
        return Task.CompletedTask;
    }

    public Task SendContractorBookingNotificationAsync(
        string contractorEmail,
        string contractorName,
        Guid bookingId,
        DateTime bookingDate,
        string customerName,
        string customerAddress)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Contractor notification sent to {contractorEmail}");
        Console.WriteLine($"  Contractor: {contractorName}");
        Console.WriteLine($"  Booking ID: {bookingId}");
        Console.WriteLine($"  Date: {bookingDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Customer: {customerName}");
        Console.WriteLine($"  Address: {customerAddress}");
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
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Payment receipt sent to {recipientEmail}");
        Console.WriteLine($"  Recipient: {recipientName}");
        Console.WriteLine($"  Booking ID: {bookingId}");
        Console.WriteLine($"  Amount: £{amount:F2}");
        Console.WriteLine($"  Method: {paymentMethod}");
        Console.WriteLine($"  Date: {paymentDate:yyyy-MM-dd HH:mm}");
        return Task.CompletedTask;
    }

    public Task SendBookingCancellationAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        string reason)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Booking cancellation sent to {recipientEmail}");
        Console.WriteLine($"  Recipient: {recipientName}");
        Console.WriteLine($"  Booking ID: {bookingId}");
        Console.WriteLine($"  Date: {bookingDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Reason: {reason}");
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
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Booking reminder sent to {recipientEmail}");
        Console.WriteLine($"  Recipient: {recipientName}");
        Console.WriteLine($"  Booking ID: {bookingId}");
        Console.WriteLine($"  Date: {bookingDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Services: {string.Join(", ", services)}");
        Console.WriteLine($"  Address: {address}");
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string textBody = null)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[MOCK EMAIL] Email sent to {recipientEmail}");
        Console.WriteLine($"  Subject: {subject}");
        Console.WriteLine($"  Body: {htmlBody}");
        return Task.CompletedTask;
    }
}
