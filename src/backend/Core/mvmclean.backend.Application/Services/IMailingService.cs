namespace mvmclean.backend.Application.Services;

public interface IMailingService
{
    /// <summary>
    /// Sends booking created email (before payment)
    /// Shows: Postcode and Telephone Number
    /// </summary>
    Task SendBookingCreatedNotificationAsync(
        string recipientEmail,
        Guid bookingId,
        string postcode,
        string telephoneNumber);

    /// <summary>
    /// Sends booking confirmation email (after successful payment)
    /// </summary>
    Task SendBookingConfirmationAsync(
        string recipientEmail,
        Guid bookingId,
        string customerName,
        string address,
        List<string> services,
        decimal totalAmount,
        DateTime bookingDate,
        string? invoiceHtml = null);

    /// <summary>
    /// Sends booking notification email to contractor
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
    /// Sends generic email with custom subject and body
    /// </summary>
    Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string? textBody = null);
}
