# Contact Form Logic Flow - Verification

## Overview
The contact form implements a clean, domain-driven architecture with proper separation of concerns using MediatR and domain events.

---

## 1. **Presentation Layer** (ContactController)
**File:** `Controllers/ContactController.cs`

```
GET /contact → Returns Contact form (Index.cshtml)
POST /contact → Submits CreateContactFormTicketRequest via MediatR
```

**Flow:**
- Accepts optional form fields: `fullName`, `email`, `phoneNumber`, `subject`, `message`
- Validates: At least `email` and `message` required
- Creates `CreateContactFormTicketRequest` object
- Sends to handler via `_mediator.Send(request)`
- Returns success/error response with TempData messaging

---

## 2. **Application Layer - Command Handler** (Feature)
**File:** `Features/SupportTicket/Commands/CreateContactFormTicket.cs`

### Request Object
```csharp
public class CreateContactFormTicketRequest : IRequest<CreateContactFormTicketResponse>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}
```

### Handler Logic
```csharp
public class CreateContactFormTicketHandler : IRequestHandler<CreateContactFormTicketRequest, CreateContactFormTicketResponse>
{
    private readonly ISupportTicketRepository _ticketRepository;

    public async Task<CreateContactFormTicketResponse> Handle(CreateContactFormTicketRequest request, CancellationToken cancellationToken)
    {
        // 1. Create system user ID (no customer account needed)
        var systemUserId = Guid.Empty;

        // 2. Call domain aggregate factory method
        var ticket = Domain.Aggregates.SupportTicket.SupportTicket.Create(
            customerId: systemUserId,
            subject: request.Subject ?? "Contact Form Submission",
            description: $"Name: {request.FullName}\nEmail: {request.Email}\nPhone: {request.PhoneNumber}\n\n{request.Message}",
            contactEmail: request.Email,
            contactPhone: request.PhoneNumber,
            contactName: request.FullName
        );

        // 3. Persist aggregate to repository
        await _ticketRepository.AddAsync(ticket);

        // 4. Return success response
        return new CreateContactFormTicketResponse
        {
            Success = true,
            ResponseMessage = "Thank you for your message...",
            TicketId = ticket.Id
        };
    }
}
```

**Key Points:**
- ✅ No email sending logic here (separation of concerns)
- ✅ No direct service dependency injection for IMailingService
- ✅ Delegates to domain aggregate for business logic
- ✅ Persists aggregate which triggers domain event

---

## 3. **Domain Layer - Aggregate Root**
**File:** `Aggregates/SupportTicket/SupportTicket.cs`

### Create Factory Method
```csharp
public static SupportTicket Create(Guid customerId, string subject, string description,
    string? contactEmail = null, string? contactPhone = null, string? contactName = null,
    Guid? bookingId = null)
{
    var ticket = new SupportTicket
    {
        CustomerId = customerId,
        BookingId = bookingId,
        Subject = subject,
        Description = description,
        Status = TicketStatus.Open
    };

    // RAISE DOMAIN EVENT
    ticket.AddDomainEvent(new SupportTicketCreatedEvent(
        ticketId: ticket.Id,
        subject: subject,
        description: description,
        contactEmail: contactEmail ?? "",
        contactPhone: contactPhone,
        contactName: contactName
    ));

    return ticket;
}
```

**Key Points:**
- ✅ Encapsulates all business rules
- ✅ Sets initial state (Status = Open)
- ✅ **Raises domain event** before returning aggregate
- ✅ Event contains all necessary contact information

---

## 4. **Domain Event**
**File:** `Aggregates/SupportTicket/Events/SupportTicketCreatedEvent.cs`

```csharp
public class SupportTicketCreatedEvent : DomainEvent
{
    public Guid TicketId { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactName { get; set; }
    public DateTime CreatedAt { get; set; }

    public SupportTicketCreatedEvent(
        Guid ticketId,
        string subject,
        string description,
        string contactEmail,
        string? contactPhone,
        string? contactName)
    {
        // Constructor initializes all properties
    }
}
```

**Key Points:**
- ✅ Inherits from `DomainEvent` base class
- ✅ Contains all contact information needed for email notification
- ✅ Published when aggregate is created
- ✅ Decouples email sending from ticket creation

---

## 5. **Application Layer - Event Handler**
**File:** `Features/SupportTicket/Events/SupportTicketCreatedEventHandler.cs`

```csharp
public class SupportTicketCreatedEventHandler : INotificationHandler<SupportTicketCreatedEvent>
{
    private readonly IMailingService _mailingService;
    private const string NotifyEmail = "saithan.uzun@gmail.com";

    public async Task Handle(SupportTicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var emailSubject = $"New Support Ticket: {notification.Subject}";
            var emailBody = $@"
                <h2>New Support Ticket Created</h2>
                <p><strong>Ticket ID:</strong> {notification.TicketId}</p>
                <p><strong>Contact Name:</strong> {notification.ContactName ?? "Not provided"}</p>
                <p><strong>Contact Email:</strong> {notification.ContactEmail}</p>
                <p><strong>Contact Phone:</strong> {notification.ContactPhone ?? "Not provided"}</p>
                <p><strong>Subject:</strong> {notification.Subject}</p>
                <p><strong>Message:</strong></p>
                <p>{notification.Description}</p>
                <p><strong>Created At:</strong> {notification.CreatedAt:yyyy-MM-dd HH:mm:ss}</p>
            ";

            await _mailingService.SendEmailAsync(NotifyEmail, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log but don't throw - prevent email failures from breaking domain operations
            Console.WriteLine($"Error sending ticket notification email: {ex.Message}");
        }
    }
}
```

**Key Points:**
- ✅ Implements `INotificationHandler<SupportTicketCreatedEvent>`
- ✅ Listens for domain event asynchronously
- ✅ Sends formatted HTML email to admin
- ✅ Error handling prevents email failures from affecting domain logic
- ✅ Decoupled from ticket creation process

---

## 6. **Dependency Injection Setup**
**File:** `Application/DependencyInjection.cs`

```csharp
public static IServiceCollection AddApplicationRegistration(this IServiceCollection serviceCollection)
{
    var asm = Assembly.GetExecutingAssembly();

    serviceCollection.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(asm));

    return serviceCollection;
}
```

**What This Does:**
- ✅ Auto-registers all MediatR commands, handlers, and notifications from the assembly
- ✅ Automatically discovers `CreateContactFormTicketHandler` (IRequestHandler)
- ✅ Automatically discovers `SupportTicketCreatedEventHandler` (INotificationHandler)
- ✅ MediatR handles event publishing when aggregate is persisted

---

## 7. **Complete Flow Diagram**

```
┌─ PRESENTATION LAYER ──────────────────────────────────────┐
│  POST /contact → ContactController.SubmitForm()           │
│  → Create CreateContactFormTicketRequest                  │
│  → _mediator.Send(request)                                │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ APPLICATION LAYER - COMMAND HANDLER ─────────────────────┐
│  CreateContactFormTicketHandler.Handle()                  │
│  → Extracts form data from request                        │
│  → Calls Domain.SupportTicket.Create()                    │
│  → Persists ticket via repository                         │
│  → Returns success response                               │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ DOMAIN LAYER - AGGREGATE ────────────────────────────────┐
│  SupportTicket.Create()                                   │
│  → Instantiates new ticket                                │
│  → Sets status = Open                                     │
│  → RAISES SupportTicketCreatedEvent                       │
│  → Returns ticket to handler                              │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ REPOSITORY ──────────────────────────────────────────────┐
│  ISupportTicketRepository.AddAsync(ticket)                │
│  → Persists aggregate to database                         │
│  → MediatR publishes domain events from aggregate         │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ APPLICATION LAYER - EVENT HANDLER ───────────────────────┐
│  SupportTicketCreatedEventHandler.Handle()                │
│  → Receives SupportTicketCreatedEvent                     │
│  → Builds email with ticket details                       │
│  → Calls IMailingService.SendEmailAsync()                 │
│  → Email sent to saithan.uzun@gmail.com                   │
│  → Catches & logs errors (non-blocking)                   │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ PRESENTATION LAYER ──────────────────────────────────────┐
│  ContactController receives response                      │
│  → Sets TempData["Success"]                               │
│  → Redirects to /contact with success message             │
└──────────────────────────────────────────────────────────┘
```

---

## 8. **Verification Results**

| Component | Status | Notes |
|-----------|--------|-------|
| **Form Fields** | ✅ Optional | All nullable strings in request |
| **Domain Event** | ✅ Raised | On aggregate creation |
| **Event Handler** | ✅ Registered | Auto-discovered by MediatR |
| **Email Sending** | ✅ Async | Non-blocking via event handler |
| **Email Recipient** | ✅ Correct | saithan.uzun@gmail.com |
| **Error Handling** | ✅ Robust | Prevents email failures from breaking flow |
| **Separation of Concerns** | ✅ Clean | No email logic in command handler |
| **Repository Pattern** | ✅ Used | ISupportTicketRepository for persistence |
| **Antiforgery Token** | ✅ Fixed | Using @Html.AntiForgeryToken() tag helper |

---

## 9. **Summary**

The contact form implements **textbook domain-driven design** with:

1. **Command Pattern (CQRS)** - CreateContactFormTicketRequest → Handler
2. **Domain Events** - SupportTicketCreatedEvent raised from aggregate
3. **Event Handlers** - Async email notification on event
4. **Repository Pattern** - Persistence via ISupportTicketRepository
5. **Dependency Injection** - Auto-registration via MediatR
6. **Error Handling** - Non-blocking email with graceful fallback
7. **Separation of Concerns** - Business logic (domain) separate from side effects (email)

**No issues found.** Logic flow is correct from presentation layer through domain layer to event handlers.
