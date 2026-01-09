# Contact Form System Refactoring - Complete Documentation

## Overview
Replaced the SupportTicket domain aggregate with a simplified Contact aggregate. The new system maintains domain-driven design principles while being specifically tailored for contact form submissions.

---

## 1. Domain Layer Changes

### New Aggregate Root: Contact
**File:** `Core/mvmclean.backend.Domain/Aggregates/Contact/Contact.cs`

```csharp
public class Contact : AggregateRoot
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string Subject { get; private set; }
    public string Message { get; private set; }
    public ContactStatus Status { get; private set; }
    
    public IReadOnlyCollection<ContactMessage> Messages => _messages.AsReadOnly();
}
```

**Factory Method:** `Create()`
- Initializes contact with provided details
- Sets initial status to `New`
- Raises `ContactCreatedEvent` domain event
- Adds initial message from customer

**Business Methods:**
- `MarkAsRead()` - Changes status to Read
- `AddAdminResponse(message, email)` - Admin adds response message, status becomes Responded
- `Resolve()` - Marks as Resolved
- `Close()` - Marks as Closed

### Contact Status Enum
**File:** `Core/mvmclean.backend.Domain/Aggregates/Contact/Enums/ContactStatus.cs`

```csharp
public enum ContactStatus
{
    New = 0,        // Initial state
    Read = 1,       // Admin has read the contact
    Responded = 2,  // Admin has responded
    Resolved = 3,   // Issue resolved
    Closed = 4      // Contact closed
}
```

### ContactMessage Entity
**File:** `Core/mvmclean.backend.Domain/Aggregates/Contact/Entities/ContactMessage.cs`

```csharp
public class ContactMessage
{
    public Guid Id { get; private set; }
    public Guid ContactId { get; private set; }
    public string Message { get; private set; }
    public string? SenderEmail { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsAdminResponse { get; private set; }
}
```

### ContactCreatedEvent Domain Event
**File:** `Core/mvmclean.backend.Domain/Aggregates/Contact/Events/ContactCreatedEvent.cs`

```csharp
public class ContactCreatedEvent : DomainEvent
{
    public Guid ContactId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### IContactRepository Interface
**File:** `Core/mvmclean.backend.Domain/Aggregates/Contact/IContactRepository.cs`

```csharp
public interface IContactRepository : IRepository<Contact>
{
    Task<Contact?> GetByIdAsync(Guid id);
    Task<IEnumerable<Contact>> GetAllAsync();
    Task AddAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(Guid id);
}
```

---

## 2. Application Layer Changes

### CreateContactFormTicket Command Handler
**File:** `Core/mvmclean.backend.Application/Features/SupportTicket/Commands/CreateContactFormTicket.cs`

**Updated to use Contact aggregate:**
```csharp
public class CreateContactFormTicketRequest : IRequest<CreateContactFormTicketResponse>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}

public class CreateContactFormTicketResponse
{
    public bool Success { get; set; }
    public required string ResponseMessage { get; set; }
    public Guid? ContactId { get; set; }  // Changed from TicketId
}

public class CreateContactFormTicketHandler : IRequestHandler<CreateContactFormTicketRequest, CreateContactFormTicketResponse>
{
    private readonly IContactRepository _contactRepository;

    public async Task<CreateContactFormTicketResponse> Handle(...)
    {
        var contact = Contact.Create(
            fullName: request.FullName ?? "Anonymous",
            email: request.Email ?? "",
            phoneNumber: request.PhoneNumber,
            subject: request.Subject ?? "Contact Form Submission",
            message: request.Message ?? ""
        );

        await _contactRepository.AddAsync(contact);
        // Domain event is automatically published via DbContext
    }
}
```

### ContactCreatedEventHandler
**File:** `Core/mvmclean.backend.Application/Features/Contact/Events/ContactCreatedEventHandler.cs`

```csharp
public class ContactCreatedEventHandler : INotificationHandler<ContactCreatedEvent>
{
    private readonly IMailingService _mailingService;
    private const string NotifyEmail = "saithan.uzun@gmail.com";

    public async Task Handle(ContactCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Sends formatted HTML email with:
        // - Contact ID
        // - Full name, email, phone
        // - Subject and message
        // - Submission timestamp
        // - Link to admin dashboard
    }
}
```

**Email Features:**
- Sends to saithan.uzun@gmail.com
- Contains all contact details
- Includes submission timestamp
- Links to admin dashboard for responses
- Non-blocking (errors don't affect domain operations)

---

## 3. Infrastructure Layer Changes

### ContactConfiguration (EF Core)
**File:** `Infrastructure/mvmclean.backend.Infrastructure/Persistence/Configurations/ContactConfiguration.cs`

```csharp
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        // Table: Contacts
        // Columns:
        //   - Id (PK)
        //   - FullName (string, 255)
        //   - Email (string, 255)
        //   - PhoneNumber (string?, 20)
        //   - Subject (string, 500)
        //   - Message (text)
        //   - Status (string - enum conversion)
        //   - CreatedAt, UpdatedAt
        //
        // Messages collection: Owned entities (inline table)
        //   - Id, ContactId
        //   - Message (text)
        //   - SenderEmail (string?, 255)
        //   - CreatedAt
        //   - IsAdminResponse (bool)
    }
}
```

### ContactRepository
**File:** `Infrastructure/mvmclean.backend.Infrastructure/Repositories/ContactRepository.cs`

```csharp
public class ContactRepository : IContactRepository
{
    public async Task<Contact?> GetByIdAsync(Guid id)
    public async Task<IEnumerable<Contact>> GetAllAsync()
    public async Task AddAsync(Contact contact)
    public async Task UpdateAsync(Contact contact)
    public async Task DeleteAsync(Guid id)
}
```

### DbContext Updates
**File:** `Infrastructure/mvmclean.backend.Infrastructure/Persistence/MVMdbContext.cs`

```csharp
// Added import
using mvmclean.backend.Domain.Aggregates.Contact;

// Added DbSet
public DbSet<Contact> Contacts { get; set; } = null!;
```

### Dependency Injection
**File:** `Infrastructure/mvmclean.backend.Infrastructure/DependencyInjection.cs`

```csharp
// Added imports
using mvmclean.backend.Domain.Aggregates.Contact;
using mvmclean.backend.Infrastructure.Repositories;

// Registered in AddInfrastructureRegistration()
serviceCollection.AddScoped<IContactRepository, ContactRepository>();
```

---

## 4. Presentation Layer Changes

### Admin Contact Controller
**File:** `Areas/Admin/Controllers/ContactController.cs`

```csharp
[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminCookie")]
[Route("admin/contacts")]
public class ContactController : Controller
{
    // GET /admin/contacts - List all contacts
    public async Task<IActionResult> Index()
    
    // GET /admin/contacts/{id} - View contact details
    public async Task<IActionResult> Detail(Guid id)
    
    // POST /admin/contacts/{id}/mark-read - Mark as read
    public async Task<IActionResult> MarkAsRead(Guid id)
    
    // POST /admin/contacts/{id}/respond - Add admin response
    public async Task<IActionResult> Respond(Guid id, string adminResponse, string adminEmail)
    
    // POST /admin/contacts/{id}/resolve - Mark as resolved
    public async Task<IActionResult> Resolve(Guid id)
    
    // POST /admin/contacts/{id}/close - Close contact
    public async Task<IActionResult> Close(Guid id)
}
```

**Admin Routes:**
- `GET /admin/contacts` - List all contacts
- `GET /admin/contacts/{id}` - View details with messages
- `POST /admin/contacts/{id}/mark-read` - Change status to Read
- `POST /admin/contacts/{id}/respond` - Add admin response message
- `POST /admin/contacts/{id}/resolve` - Change status to Resolved
- `POST /admin/contacts/{id}/close` - Change status to Closed

### Admin Views

#### Index View (`Areas/Admin/Views/Contact/Index.cshtml`)
**Features:**
- Table of all contacts
- Columns: Name, Email, Phone, Subject, Status, Submitted, Actions
- Status badges (color-coded):
  - New (red)
  - Read (yellow)
  - Responded (blue)
  - Resolved (green)
  - Closed (gray)
- View button links to detail page
- Sorted by newest first

#### Detail View (`Areas/Admin/Views/Contact/Detail.cshtml`)
**Features:**
- Contact information display
- Status badge and quick status change buttons
- Timeline view of all messages:
  - Customer messages (blue)
  - Admin responses (blue)
  - Timestamps and sender email
- Admin response form (if not closed)
- Quick actions sidebar:
  - Mark as Resolved button
  - Close Contact button
- Summary section:
  - Total messages count
  - Admin responses count
  - Days open

---

## 5. Complete Request Flow

```
┌─ PRESENTATION LAYER ──────────────────────────────────────┐
│  POST /contact (ContactController)                        │
│  → Validates email and message                            │
│  → Creates CreateContactFormTicketRequest                 │
│  → Sends via _mediator.Send()                             │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ APPLICATION LAYER - COMMAND HANDLER ─────────────────────┐
│  CreateContactFormTicketHandler                           │
│  → Receives form data                                     │
│  → Calls Contact.Create()                                 │
│  → Awaits _contactRepository.AddAsync()                   │
│  → Returns success response                               │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ DOMAIN LAYER ────────────────────────────────────────────┐
│  Contact.Create()                                         │
│  → Instantiates new Contact                               │
│  → Sets status = ContactStatus.New                        │
│  → Adds initial message                                   │
│  → RAISES ContactCreatedEvent                             │
│  → Returns aggregate                                      │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ INFRASTRUCTURE LAYER ────────────────────────────────────┐
│  ContactRepository.AddAsync()                             │
│  → Persists contact to Contacts table                     │
│  → Calls DbContext.SaveChangesAsync()                     │
│  → MediatR publishes domain events                        │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ APPLICATION LAYER - EVENT HANDLER ───────────────────────┐
│  ContactCreatedEventHandler (INotificationHandler)        │
│  → Receives ContactCreatedEvent                           │
│  → Builds HTML email with details                         │
│  → Calls IMailingService.SendEmailAsync()                 │
│  → Email sent to saithan.uzun@gmail.com                   │
│  → Catches errors (non-blocking)                          │
└──────────────────────────────────────────────────────────┘
                         ↓
┌─ PRESENTATION LAYER ──────────────────────────────────────┐
│  ContactController returns response                       │
│  → Success: Sets TempData, redirects                      │
│  → Error: Shows error message on form                     │
└──────────────────────────────────────────────────────────┘
```

---

## 6. Admin Dashboard Flow

```
┌─ ADMIN DASHBOARD (Requires Admin Authentication) ────────┐
│                                                          │
│  GET /admin/contacts → Lists all contacts               │
│    ↓                                                     │
│  GET /admin/contacts/{id} → View details                │
│    ↓                                                     │
│  [Mark as Read] → POST mark-read → Update status        │
│  [Send Response] → POST respond → Add message            │
│  [Mark Resolved] → POST resolve → Update status          │
│  [Close Contact] → POST close → Update status            │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## 7. Database Schema

### Contacts Table
```sql
CREATE TABLE "Contacts" (
    "Id" uuid PRIMARY KEY,
    "FullName" varchar(255) NOT NULL,
    "Email" varchar(255) NOT NULL,
    "PhoneNumber" varchar(20),
    "Subject" varchar(500) NOT NULL,
    "Message" text NOT NULL,
    "Status" varchar NOT NULL,  -- Enum as string
    "CreatedAt" timestamp NOT NULL,
    "UpdatedAt" timestamp,
    "IsDeleted" boolean DEFAULT false
);

-- Messages stored as owned entities (inline)
-- Additional fields in Contacts table:
--   _messages_Id
--   _messages_Message
--   _messages_SenderEmail
--   _messages_CreatedAt
--   _messages_IsAdminResponse
```

---

## 8. Key Features

✅ **Domain-Driven Design**
- Contact aggregate encapsulates all business logic
- Domain events for email notifications
- Repository pattern for persistence

✅ **Contact Management**
- Track status progression: New → Read → Responded → Resolved → Closed
- Store messages and admin responses in timeline
- Multiple admin responses possible

✅ **Email Notifications**
- Automatic email to admin when contact submitted
- Asynchronous, non-blocking
- Full contact details included in email

✅ **Admin Dashboard**
- View all contacts with status filtering
- Detailed view with message timeline
- Quick status management actions
- Admin response capability

✅ **Optional Form Fields**
- Only email and message truly required
- Name, phone, subject have defaults
- Flexible for partial submissions

✅ **Authentication**
- Admin area requires AdminCookie authentication
- Protects contact list and management actions

---

## 9. Breaking Changes

❌ **Removed:**
- SupportTicket domain aggregate (still exists for legacy support tickets)
- CreateContactFormTicket no longer references SupportTicket

✅ **New:**
- Contact aggregate (contact-form specific)
- ContactCreatedEvent (contact-form specific)
- IContactRepository
- Admin Contact Controller
- Admin Contact Views

---

## 10. Migration Required

Run database migration to create Contacts table:

```bash
dotnet ef migrations add AddContactAggregate --project Infrastructure
dotnet ef database update --project Infrastructure
```

---

## 11. Configuration Checklist

✅ Domain layer - Contact aggregate created
✅ Domain events - ContactCreatedEvent created
✅ Application layer - Command handler updated
✅ Application layer - Event handler created
✅ Infrastructure layer - Repository created
✅ Infrastructure layer - DbContext updated
✅ Infrastructure layer - EF Core configuration added
✅ Dependency injection - ContactRepository registered
✅ Presentation layer - Admin controller created
✅ Presentation layer - Admin views created
✅ Email notifications - Event handler sends emails
✅ Admin routes - Configured in Program.cs

---

## 12. Usage Example

### Create Contact (from form)
```csharp
var request = new CreateContactFormTicketRequest
{
    FullName = "John Doe",
    Email = "john@example.com",
    PhoneNumber = "07123456789",
    Subject = "Service Inquiry",
    Message = "I need professional cleaning..."
};

var response = await _mediator.Send(request);
// ContactCreatedEvent automatically published → Email sent to admin
```

### Respond in Admin Dashboard
```csharp
var contact = await _contactRepository.GetByIdAsync(contactId);
contact.AddAdminResponse("We'll get back to you soon", "admin@example.com");
await _contactRepository.UpdateAsync(contact);
// Contact status changes to Responded
// Message added to timeline
```

---

## Summary

The refactored system replaces the generic SupportTicket with a focused Contact aggregate, maintaining clean architecture principles while providing a specialized system for contact form submissions. The admin dashboard enables efficient management of customer inquiries with status tracking and response capabilities.
