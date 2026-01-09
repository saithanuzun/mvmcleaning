using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mvmclean.backend.Application.Services;
using Stripe;
using Stripe.Checkout;

namespace mvmclean.backend.Infrastructure.Services;

/// <summary>
/// Stripe payment processing service implementation
/// Handles creation and verification of Stripe payment links for bookings
/// </summary>
public class StripeService : IStripeService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IConfiguration configuration, ILogger<StripeService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Initialize Stripe with API key from configuration
        var stripeSecretKey = _configuration["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(stripeSecretKey))
        {
            throw new InvalidOperationException("Stripe:SecretKey is not configured in appsettings");
        }
        StripeConfiguration.ApiKey = stripeSecretKey;
    }

    /// <summary>
    /// Creates a Stripe payment link for a booking
    /// Payment links allow customers to complete payment via Stripe Checkout
    /// </summary>
    public async Task<string> CreatePaymentLinkAsync(
        Guid bookingId,
        decimal amount,
        string currency,
        string description,
        string successUrl,
        string cancelUrl)
    {
        try
        {
            _logger.LogInformation("Creating Stripe payment link for booking {BookingId}, amount: {Amount} {Currency}", 
                bookingId, amount, currency);

            // Convert amount to smallest currency unit (pence for GBP, cents for USD, etc.)
            var amountInSmallestUnit = (long)(amount * 100);

            // Create checkout session options
            var options = new SessionCreateOptions
            {
                // Payment method
                PaymentMethodTypes = new List<string> { "card" },
                
                // Mode: payment (one-time payment)
                Mode = "payment",
                
                // Line items (what customer is paying for)
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency.ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = description,
                                Description = $"Booking ID: {bookingId}"
                            },
                            UnitAmount = amountInSmallestUnit
                        },
                        Quantity = 1
                    }
                },
                
                // Redirect URLs after payment
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                
                // Store booking ID as metadata for later retrieval
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId.ToString() }
                }
            };

            // Create the session
            var service = new SessionService();
            var session = await service.CreateAsync(options);

            _logger.LogInformation("Successfully created Stripe checkout session {SessionId} for booking {BookingId}", 
                session.Id, bookingId);

            // Return the payment URL
            return session.Url;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error creating payment link for booking {BookingId}: {StripeError}", 
                bookingId, ex.StripeResponse);
            throw new InvalidOperationException($"Failed to create Stripe payment link: {ex.StripeResponse}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating Stripe payment link for booking {BookingId}", bookingId);
            throw new InvalidOperationException($"Failed to create Stripe payment link: {ex.Message}");
        }
    }

    /// <summary>
    /// Verifies that a Stripe checkout session completed successfully
    /// </summary>
    public async Task<bool> VerifyPaymentAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Verifying Stripe payment for session {SessionId}", sessionId);

            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            // Check if payment was successful
            bool isSuccessful = session.PaymentStatus == "paid" || 
                               session.PaymentStatus == "no_payment_required";

            _logger.LogInformation("Stripe session {SessionId} payment status: {PaymentStatus}", 
                sessionId, session.PaymentStatus);

            return isSuccessful;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error verifying payment for session {SessionId}: {StripeError}", 
                sessionId, ex.StripeResponse);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error verifying payment for session {SessionId}", sessionId);
            return false;
        }
    }

    /// <summary>
    /// Retrieves detailed payment information for a Stripe checkout session
    /// </summary>
    public async Task<PaymentDetails> GetPaymentDetailsAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Retrieving payment details for session {SessionId}", sessionId);

            var service = new SessionService();
            var session = await service.GetAsync(sessionId, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            var paymentDetails = new PaymentDetails
            {
                SessionId = session.Id,
                PaymentStatus = session.PaymentStatus ?? "unknown",
                AmountPaid = (session.AmountTotal ?? 0) / 100m, // Convert from smallest unit
                Currency = session.Currency?.ToUpper() ?? "GBP",
                PaymentIntentId = session.PaymentIntentId ?? string.Empty,
                PaymentDate = session.Created
            };

            _logger.LogInformation("Retrieved payment details for session {SessionId}: Status={PaymentStatus}, Amount={AmountPaid}{Currency}", 
                sessionId, paymentDetails.PaymentStatus, paymentDetails.AmountPaid, paymentDetails.Currency);

            return paymentDetails;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error retrieving payment details for session {SessionId}: {StripeError}", 
                sessionId, ex.StripeResponse);
            throw new InvalidOperationException($"Failed to retrieve payment details: {ex.StripeResponse}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving payment details for session {SessionId}", sessionId);
            throw new InvalidOperationException($"Failed to retrieve payment details: {ex.Message}");
        }
    }
}
