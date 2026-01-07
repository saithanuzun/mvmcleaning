namespace mvmclean.backend.Application.Services;

public interface IStripeService
{
    /// <summary>
    /// Creates a Stripe payment link for a booking
    /// </summary>
    /// <param name="bookingId">The booking ID</param>
    /// <param name="amount">Amount in the smallest currency unit (e.g., cents for USD, pence for GBP)</param>
    /// <param name="currency">Currency code (e.g., "gbp", "usd")</param>
    /// <param name="description">Description for the payment</param>
    /// <param name="successUrl">URL to redirect after successful payment</param>
    /// <param name="cancelUrl">URL to redirect if payment is cancelled</param>
    /// <returns>Stripe payment link URL</returns>
    Task<string> CreatePaymentLinkAsync(
        Guid bookingId,
        decimal amount,
        string currency,
        string description,
        string successUrl,
        string cancelUrl);

    /// <summary>
    /// Verifies a payment session to ensure it was successful
    /// </summary>
    /// <param name="sessionId">Stripe session ID</param>
    /// <returns>True if payment was successful, false otherwise</returns>
    Task<bool> VerifyPaymentAsync(string sessionId);

    /// <summary>
    /// Gets payment details for a session
    /// </summary>
    /// <param name="sessionId">Stripe session ID</param>
    /// <returns>Payment details including amount, status, etc.</returns>
    Task<PaymentDetails> GetPaymentDetailsAsync(string sessionId);
}

public class PaymentDetails
{
    public string SessionId { get; set; }
    public string PaymentStatus { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; }
    public string PaymentIntentId { get; set; }
    public DateTime? PaymentDate { get; set; }
}
