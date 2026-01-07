using mvmclean.backend.Application.Services;

namespace mvmclean.backend.Infrastructure.Services;

/// <summary>
/// Stub implementation of Stripe service - to be implemented with actual Stripe SDK
/// </summary>
public class StripeService : IStripeService
{
    public Task<string> CreatePaymentLinkAsync(
        Guid bookingId,
        decimal amount,
        string currency,
        string description,
        string successUrl,
        string cancelUrl)
    {
        // TODO: Implement actual Stripe payment link creation
        // For now, return a mock payment URL
        var mockPaymentUrl = $"https://checkout.stripe.com/mock/{bookingId}";
        return Task.FromResult(mockPaymentUrl);
    }

    public Task<bool> VerifyPaymentAsync(string sessionId)
    {
        // TODO: Implement actual Stripe payment verification
        // For now, return true for testing purposes
        return Task.FromResult(true);
    }

    public Task<Application.Services.PaymentDetails> GetPaymentDetailsAsync(string sessionId)
    {
        // TODO: Implement actual Stripe payment details retrieval
        // For now, return mock payment details
        return Task.FromResult(new Application.Services.PaymentDetails
        {
            SessionId = sessionId,
            PaymentStatus = "paid",
            AmountPaid = 0,
            Currency = "gbp",
            PaymentIntentId = "pi_mock",
            PaymentDate = DateTime.UtcNow
        });
    }
}
