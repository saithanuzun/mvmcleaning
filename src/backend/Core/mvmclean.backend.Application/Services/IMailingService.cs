namespace mvmclean.backend.Application.Services;

public interface IMailingService
{
    /// <summary>
    /// Sends booking confirmation email to customer
    /// </summary>
    Task SendBookingConfirmationAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        List<string> services,
        decimal totalAmount);

    /// <summary>
    /// Sends booking confirmation email to contractor
    /// </summary>
    Task SendContractorBookingNotificationAsync(
        string contractorEmail,
        string contractorName,
        Guid bookingId,
        DateTime bookingDate,
        string customerName,
        string customerAddress);

    /// <summary>
    /// Sends payment receipt email
    /// </summary>
    Task SendPaymentReceiptAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        decimal amount,
        string paymentMethod,
        DateTime paymentDate);

    /// <summary>
    /// Sends booking cancellation email
    /// </summary>
    Task SendBookingCancellationAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        string reason);

    /// <summary>
    /// Sends booking reminder email
    /// </summary>
    Task SendBookingReminderAsync(
        string recipientEmail,
        string recipientName,
        Guid bookingId,
        DateTime bookingDate,
        List<string> services,
        string address);

    /// <summary>
    /// Sends generic email
    /// </summary>
    Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string textBody = null);
}
