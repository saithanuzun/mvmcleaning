# Stripe Integration Guide

## Architecture Overview

The application integrates Stripe payment processing with a clean separation of concerns:

```
React Frontend (BookingPage)
    ↓
    └→ api.booking.complete(request)
            ↓
            └→ BookingController.Complete()
                    ↓
                    └→ CompleteBookingHandler
                            ↓
                            └→ StripeService.CreatePaymentLinkAsync()
                                    ↓
                                    └→ Stripe API → Payment Link URL
                                    
User redirected to Stripe Checkout
    ↓
    ├→ Payment Success → Redirect to /payment-success
    └→ Payment Cancelled → Redirect to /payment-failed
```

## Components

### 1. **StripeService** (Infrastructure Layer)
**File:** `src/backend/Infrastructure/mvmclean.backend.Infrastructure/Services/StripeService.cs`

**Responsibilities:**
- Initialize Stripe SDK with API key from configuration
- Create Stripe Checkout sessions for payment
- Verify payment completion
- Retrieve payment details and session information

**Key Methods:**

#### `CreatePaymentLinkAsync()`
- Creates a Stripe Checkout session for booking payment
- Converts amount to smallest currency unit (pence for GBP, cents for USD)
- Stores booking ID in session metadata
- Returns the Stripe Checkout URL for redirect

```csharp
// Usage in CompleteBooking handler
paymentLink = await _stripeService.CreatePaymentLinkAsync(
    bookingId,           // Booking identifier
    totalAmount,         // Total amount in pounds (e.g., 150.50)
    "gbp",              // Currency code
    "Booking payment",  // Description
    successUrl,         // Redirect on success
    cancelUrl           // Redirect on cancel
);
```

#### `VerifyPaymentAsync()`
- Checks if a Stripe checkout session was completed successfully
- Returns true if `paymentStatus == "paid"` or `"no_payment_required"`
- Used for server-side payment verification

#### `GetPaymentDetailsAsync()`
- Retrieves full payment details from a Stripe session
- Includes amount, currency, payment intent ID, and timestamp
- Expands payment_intent for detailed transaction info

### 2. **IStripeService Interface** (Application Layer)
**File:** `src/backend/Core/mvmclean.backend.Application/Services/IStripeService.cs`

Defines the contract for payment processing service:
- `Task<string> CreatePaymentLinkAsync()` - Creates payment session
- `Task<bool> VerifyPaymentAsync(string sessionId)` - Verifies payment
- `Task<PaymentDetails> GetPaymentDetailsAsync(string sessionId)` - Gets payment info

### 3. **PaymentDetails Class** (Application Layer)
Data transfer object for payment information:
```csharp
public class PaymentDetails
{
    public string SessionId { get; set; }
    public string PaymentStatus { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; }
    public string PaymentIntentId { get; set; }
    public DateTime? PaymentDate { get; set; }
}
```

### 4. **CompleteBooking Command Handler**
**File:** `src/backend/Core/mvmclean.backend.Application/Features/Booking/Commands/CompleteBooking.cs`

Payment flow:
1. Validates booking exists and not already completed
2. **If Cash Payment:**
   - Directly confirms booking
   - Returns booking ID (no payment URL)
   
3. **If Card Payment:**
   - Calls `StripeService.CreatePaymentLinkAsync()`
   - Returns Stripe Checkout URL
   - User redirected to Stripe for payment

## Configuration

### appsettings.json Setup

Add Stripe configuration:

```json
{
  "Stripe": {
    "SecretKey": "sk_test_YOUR_KEY_HERE",
    "PublishableKey": "pk_test_YOUR_KEY_HERE",
    "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET"
  },
  
  "App": {
    "BaseUrl": "http://localhost:3000"
  }
}
```

### Getting Stripe API Keys

1. **Create Stripe Account** → [stripe.com](https://stripe.com)
2. **Go to Dashboard** → Settings → API Keys
3. **Copy Keys:**
   - **Secret Key** (sk_...) - Use in backend
   - **Publishable Key** (pk_...) - Use in frontend
   - **Webhook Secret** (whsec_...) - For webhook verification

### Development vs Production

**Development (localhost):**
```json
"App": {
  "BaseUrl": "http://localhost:3000"
}
```

**Production:**
```json
"App": {
  "BaseUrl": "https://yourdomain.com"
}
```

## Payment Flow

### Successful Payment Flow

```
1. Customer fills booking details → BookingPage
2. Selects "Card" payment method
3. Clicks "Complete Booking"
4. Backend creates Stripe Checkout session
5. React redirects to Stripe Checkout URL
6. Customer enters card details
7. Payment successful
8. Stripe redirects to http://localhost:3000/shop/payment-success
9. PaymentSuccessPage displays confirmation
```

### Failed/Cancelled Payment Flow

```
1. Customer on Stripe Checkout page
2. Clicks "Back" or payment fails
3. Stripe redirects to http://localhost:3000/shop/payment-failed
4. PaymentFailedPage displays error
5. Customer can retry payment or start over
```

### Cash Payment Flow

```
1. Customer fills booking details → BookingPage
2. Selects "Cash" payment method
3. Clicks "Complete Booking"
4. Backend immediately confirms booking
5. No Stripe interaction
6. React redirects to http://localhost:3000/shop/payment-success
7. Booking reference displayed
```

## Frontend Integration

### BookingPage.jsx

```javascript
// Handle card payment
if (paymentMethod === 'card') {
    const response = await api.booking.complete(bookingRequest);
    // Store data in localStorage for success page
    localStorage.setItem('pending_booking_id', response.data.bookingId);
    localStorage.setItem('booking_data', JSON.stringify({...}));
    // Redirect to Stripe Checkout
    window.location.href = response.data.paymentUrl;
}

// Handle cash payment
if (paymentMethod === 'cash') {
    // Stored in localStorage
    localStorage.setItem('pending_booking_id', response.data.bookingId);
    // Redirect directly to success page
    navigate('/payment-success');
}
```

### PaymentSuccessPage.jsx

```javascript
// Read booking data from localStorage
useEffect(() => {
    const bookingId = localStorage.getItem('pending_booking_id');
    const bookingData = JSON.parse(localStorage.getItem('booking_data'));
    // Display confirmation
    // Clear after 5 seconds
}, []);
```

### PaymentFailedPage.jsx

```javascript
// Display payment error
// Provide retry button
// Provide start-over button
```

## Error Handling

### StripeService Errors

1. **StripeException** - API errors from Stripe
   - Logged with error details
   - Thrown as InvalidOperationException to caller
   
2. **Configuration Errors** - Missing API keys
   - Throws InvalidOperationException on initialization
   
3. **Network Errors** - Connection issues
   - Logged and thrown as InvalidOperationException

### Logging

All operations logged using ILogger:
```csharp
_logger.LogInformation("Creating Stripe payment link for booking {BookingId}", bookingId);
_logger.LogError("Stripe API error: {StripeError}", ex.StripeResponse?.Error?.Message);
```

## Testing

### Stripe Test Mode

Use Stripe test cards for development:

- **Successful Payment:**
  - Card: `4242 4242 4242 4242`
  - Exp: Any future date
  - CVC: Any 3 digits
  
- **Payment Declined:**
  - Card: `4000 0000 0000 0002`
  - Exp: Any future date
  - CVC: Any 3 digits
  
- **Requires Authentication:**
  - Card: `4000 0025 0000 3155`

### Testing Payment Flow

1. Start application
2. Create booking with test data
3. Select "Card" payment method
4. Use Stripe test card
5. Verify redirect to success/failed page
6. Check localStorage for booking data

## NuGet Dependencies

- **Stripe.net** (v50.2.0+) - Official Stripe C# SDK

## Security Considerations

1. **Never commit API keys** to repository
2. **Use User Secrets** in development:
   ```bash
   dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
   ```
3. **Environment variables** in production
4. **HTTPS only** for production URLs
5. **Validate webhook signatures** when implementing webhooks
6. **Store payment intent ID** for idempotency

## Next Steps: Webhooks

To handle asynchronous payment events (refunds, disputes, etc.):

1. **Create Webhook Endpoint** in backend:
   ```csharp
   [HttpPost("webhooks/stripe")]
   public async Task<IActionResult> HandleStripeWebhook()
   {
       // Verify signature
       // Process event
       // Return 200 OK
   }
   ```

2. **Configure Webhook** in Stripe Dashboard:
   - Events to listen: `charge.succeeded`, `charge.failed`, `charge.refunded`
   - Endpoint URL: `https://yourdomain.com/api/webhooks/stripe`

3. **Implement Event Handlers** for:
   - Payment succeeded
   - Payment failed
   - Refund processed
   - Dispute opened

## Production Checklist

- [ ] Replace test API keys with production keys
- [ ] Update `App:BaseUrl` to production domain
- [ ] Enable HTTPS for all URLs
- [ ] Implement webhook handling
- [ ] Set up email notifications
- [ ] Test end-to-end with real cards (or use Stripe testing)
- [ ] Monitor Stripe Dashboard for failed payments
- [ ] Configure logging to production sink
- [ ] Set up alerts for payment failures
- [ ] Document support process for payment issues

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "API key not configured" | Check appsettings.json `Stripe:SecretKey` is set |
| Payment link returns null | Verify Stripe API key is valid and active |
| Redirect URL not working | Check `App:BaseUrl` matches React app URL |
| Stripe session not found | Session may have expired, check session ID format |
| Logging not appearing | Verify ILogger<StripeService> is injected and configured |

## References

- Stripe .NET SDK: https://github.com/stripe/stripe-dotnet
- Stripe API Docs: https://stripe.com/docs/api
- Checkout Sessions: https://stripe.com/docs/checkout/overview
- Test Cards: https://stripe.com/docs/testing#cards
