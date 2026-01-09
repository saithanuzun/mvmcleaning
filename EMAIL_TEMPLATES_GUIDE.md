# Email Templates & Integration Guide

## Overview

The email system provides professional, formatted email templates for all booking-related communications:

1. **Booking Created Email** - Sent immediately when booking is created (before payment)
2. **Booking Confirmed Email** - Sent after successful payment (card) or immediately (cash)

## Email Templates

### 1. Booking Created Email Template

**File:** `src/backend/Infrastructure/mvmclean.backend.Infrastructure/MailingService/EmailTemplate.cs`

**Purpose:** Sent when customer creates a booking but before payment is processed

**Data Displayed:**
- ✅ Booking Reference Number (first 8 characters of Booking ID)
- ✅ Customer Name
- ✅ **Postcode**
- ✅ **Telephone Number**
- ✅ Services booked
- ✅ Total amount
- ✅ Next steps information

**Class:** `BookingCreatedEmailTemplate`

```csharp
var template = new BookingCreatedEmailTemplate(
    bookingId: "550e8400-e29b-41d4-a716-446655440000",
    customerName: "John Smith",
    postcode: "SW1A 1AA",
    telephoneNumber: "020 1234 5678",
    services: new List<string> { "General Cleaning", "Window Cleaning" },
    totalAmount: 150.50m
);

// Access template content:
string subject = template.Subject;
string htmlBody = template.HtmlBody;
string plainText = template.PlainTextBody;
```

**Template Features:**
- Professional gradient header (blue & cyan)
- Clear postcode and telephone display
- Service list with pricing
- "What happens next?" section
- Plain text fallback for email clients

---

### 2. Booking Confirmed Email Template

**File:** `src/backend/Infrastructure/mvmclean.backend.Infrastructure/MailingService/EmailTemplate.cs`

**Purpose:** Sent after successful payment (card payment or cash payment)

**Data Displayed:**
- ✅ Booking Reference Number
- ✅ Payment confirmation status
- ✅ Appointment date and time
- ✅ Customer details (name, email, phone, postcode, address)
- ✅ Services confirmed
- ✅ Total amount paid
- ✅ Payment method
- ✅ What happens next (contractor contact, reminders, etc.)

**Class:** `BookingConfirmedEmailTemplate`

```csharp
var template = new BookingConfirmedEmailTemplate(
    bookingId: "550e8400-e29b-41d4-a716-446655440000",
    customerName: "John Smith",
    customerEmail: "john@example.com",
    postcode: "SW1A 1AA",
    telephoneNumber: "020 1234 5678",
    address: "123 Main Street, London",
    services: new List<string> { "General Cleaning", "Window Cleaning" },
    totalAmount: 150.50m,
    bookingDate: DateTime.Now.AddDays(3),
    timeSlot: "09:00 - 11:00",
    paymentMethod: "Card"
);
```

**Template Features:**
- Success badge "✓ Payment Received"
- Green highlight for appointment details
- Complete customer information section
- Detailed service breakdown
- Payment summary with method
- 4-step "What happens next?" section
- Professional footer with support info

---

## Email Service Integration

### MailingService Implementation

**File:** `src/backend/Infrastructure/mvmclean.backend.Infrastructure/Services/MailingService.cs`

**Responsibilities:**
- Generate email templates from domain data
- Queue emails for sending
- Provide logging of email operations
- Handle errors gracefully

**Methods:**

#### SendBookingCreatedNotificationAsync()
```csharp
await _mailingService.SendBookingCreatedNotificationAsync(
    recipientEmail: "customer@example.com",
    recipientName: "John Smith",
    bookingId: Guid.NewGuid(),
    postcode: "SW1A 1AA",
    telephoneNumber: "020 1234 5678",
    services: new List<string> { "Cleaning", "Laundry" },
    totalAmount: 150.50m
);
```

#### SendBookingConfirmationAsync()
```csharp
await _mailingService.SendBookingConfirmationAsync(
    recipientEmail: "customer@example.com",
    recipientName: "John Smith",
    bookingId: Guid.NewGuid(),
    bookingDate: DateTime.Now.AddDays(3),
    services: new List<string> { "Cleaning", "Laundry" },
    totalAmount: 150.50m
);
```

#### SendEmailAsync()
Generic method for custom emails:
```csharp
await _mailingService.SendEmailAsync(
    recipientEmail: "customer@example.com",
    subject: "Custom Email Subject",
    htmlBody: "<html>...</html>",
    textBody: "Plain text version"
);
```

---

## Email Flow in Booking Process

### For Cash Payment:

```
1. Customer completes booking form
        ↓
2. CompleteBooking handler invoked
        ↓
3. Booking created in database
        ↓
4. Send "Booking Created" email (shows postcode & phone)
        ↓
5. Mark booking as paid (cash payment)
        ↓
6. Send "Booking Confirmed" email immediately
        ↓
7. Customer sees success page
```

### For Card Payment:

```
1. Customer completes booking form
        ↓
2. CompleteBooking handler invoked
        ↓
3. Booking created in database
        ↓
4. Send "Booking Created" email (shows postcode & phone)
        ↓
5. Create Stripe payment link
        ↓
6. Redirect to Stripe checkout
        ↓
7. Customer completes payment
        ↓
8. Stripe redirects to success page
        ↓
9. Send "Booking Confirmed" email
        ↓
10. Customer sees success page with booking details
```

---

## Integration with CompleteBooking Handler

**File:** `src/backend/Core/mvmclean.backend.Application/Features/Booking/Commands/CompleteBooking.cs`

The handler now includes email sending:

```csharp
public class CompleteBookingHandler : IRequestHandler<CompleteBookingRequest, CompleteBookingResponse>
{
    private readonly IMailingService _mailingService;

    public async Task<CompleteBookingResponse> Handle(CompleteBookingRequest request, CancellationToken cancellationToken)
    {
        // ... booking logic ...

        // Send booking created notification
        await _mailingService.SendBookingCreatedNotificationAsync(
            request.CustomerEmail,
            request.CustomerName,
            bookingId,
            request.Postcode,
            request.CustomerPhone,
            serviceNames,
            totalAmount
        );

        // Send confirmation if cash payment
        if (paymentType == PaymentType.Cash)
        {
            await _mailingService.SendBookingConfirmationAsync(
                request.CustomerEmail,
                request.CustomerName,
                bookingId,
                scheduling.StartTime,
                serviceNames,
                totalAmount
            );
        }
    }
}
```

---

## Email Provider Integration

### Current Status (Mock Implementation)

Currently, emails are logged to console for development/testing.

### To Integrate with SendGrid:

**1. Install NuGet Package:**
```bash
dotnet add package SendGrid
```

**2. Update appsettings.json:**
```json
{
  "SendGrid": {
    "ApiKey": "SG.your_sendgrid_api_key_here",
    "FromEmail": "noreply@mvmclean.com",
    "FromName": "MVM Clean"
  }
}
```

**3. Update MailingService:**
```csharp
using SendGrid;
using SendGrid.Helpers.Mail;

public class MailingService : IMailingService
{
    private readonly SendGridClient _sendGridClient;
    private readonly IConfiguration _config;

    public MailingService(IConfiguration config)
    {
        _config = config;
        var apiKey = config["SendGrid:ApiKey"];
        _sendGridClient = new SendGridClient(apiKey);
    }

    public async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string textBody = null)
    {
        var from = new EmailAddress(
            _config["SendGrid:FromEmail"],
            _config["SendGrid:FromName"]
        );
        var to = new EmailAddress(recipientEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, textBody, htmlBody);
        
        await _sendGridClient.SendEmailAsync(msg);
    }
}
```

### To Integrate with SMTP:

```csharp
using System.Net;
using System.Net.Mail;

public async Task SendEmailAsync(
    string recipientEmail,
    string subject,
    string htmlBody,
    string textBody = null)
{
    using (var client = new SmtpClient(_config["Smtp:Host"]))
    {
        client.Port = int.Parse(_config["Smtp:Port"]);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(
            _config["Smtp:Username"],
            _config["Smtp:Password"]
        );

        var message = new MailMessage(
            _config["Smtp:FromEmail"],
            recipientEmail,
            subject,
            htmlBody
        )
        {
            IsBodyHtml = true,
            AlternateViews = { AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain") }
        };

        await client.SendMailAsync(message);
    }
}
```

---

## Logging & Monitoring

All email operations are logged with structured logging:

```csharp
_logger.LogInformation(
    "Booking created email sent to {Email} for booking {BookingId}",
    recipientEmail, bookingId);

_logger.LogError(
    ex,
    "Failed to send email to {Email} for booking {BookingId}",
    recipientEmail, bookingId);
```

### Monitoring Checklist:

- [ ] All booking created emails have postcode and telephone
- [ ] All booking confirmed emails have complete details
- [ ] No email failures block booking completion
- [ ] Email send errors are logged with full context
- [ ] Email templates render correctly in different clients
- [ ] Links in emails work properly
- [ ] Plain text fallback displays correctly

---

## Email Template Customization

### Customize Branding:

In `EmailTemplate.cs`, update:
```csharp
// Colors
background: linear-gradient(135deg, #194376 0%, #46C6CE 100%);  // Change to your brand colors
color: #194376;

// Company info
Company Name: MVM Clean
Email: support@mvmclean.com
Phone: 020 XXXX XXXX
```

### Customize Content:

Modify template strings in:
- `BookingCreatedEmailTemplate.GenerateTemplate()`
- `BookingConfirmedEmailTemplate.GenerateTemplate()`

Add additional sections like:
- Discount codes
- Testimonials
- Related services
- Social media links

---

## Testing Email Templates

### Unit Test Example:

```csharp
[Test]
public void BookingCreatedTemplate_ContainsPostcodeAndTelephone()
{
    var template = new BookingCreatedEmailTemplate(
        "550e8400-e29b-41d4-a716-446655440000",
        "John Smith",
        "SW1A 1AA",
        "020 1234 5678",
        new List<string> { "Cleaning" },
        150.50m
    );

    Assert.That(template.HtmlBody, Does.Contain("SW1A 1AA"));
    Assert.That(template.HtmlBody, Does.Contain("020 1234 5678"));
    Assert.That(template.Subject, Does.Contain("550e8400-e29b".Substring(0, 8).ToUpper()));
}

[Test]
public void BookingConfirmedTemplate_ContainsAllDetails()
{
    var template = new BookingConfirmedEmailTemplate(
        "550e8400-e29b-41d4-a716-446655440000",
        "John Smith",
        "john@example.com",
        "SW1A 1AA",
        "020 1234 5678",
        "123 Main Street",
        new List<string> { "Cleaning" },
        150.50m,
        DateTime.Now.AddDays(3),
        "09:00 - 11:00",
        "Card"
    );

    Assert.That(template.HtmlBody, Does.Contain("john@example.com"));
    Assert.That(template.HtmlBody, Does.Contain("£150.50"));
    Assert.That(template.HtmlBody, Does.Contain("Payment Received"));
}
```

---

## Production Checklist

- [ ] Update SendGrid API key in production environment variables
- [ ] Update from email address to your domain (@mvmclean.com)
- [ ] Configure SPF/DKIM records for email authentication
- [ ] Set up email unsubscribe lists (if needed)
- [ ] Test email delivery with real test accounts
- [ ] Set up email logging/monitoring (SendGrid dashboard)
- [ ] Configure reply-to email address
- [ ] Add unsubscribe link (required by law in some regions)
- [ ] Set up bounce handling
- [ ] Monitor email deliverability rates
- [ ] Test email rendering in major email clients
- [ ] Update contact support information
- [ ] Configure email templates in SendGrid dashboard (optional)

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Emails not sending | Check SendGrid API key in configuration |
| Templates not rendering | Verify CSS is compatible with email clients |
| Postcode not showing | Ensure `request.Postcode` is passed correctly |
| Phone number missing | Verify `request.CustomerPhone` is not empty |
| HTML rendering issues | Test in major email clients (Gmail, Outlook, etc.) |
| Emails going to spam | Check SPF/DKIM records and email authentication |

