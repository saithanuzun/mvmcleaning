# Admin Contact Management Features - Complete Implementation

## Overview
Created comprehensive admin dashboard features for contact management using MediatR queries and commands. The system separates business logic from controller logic through the application layer.

---

## 1. Features Layer (Application)

### Queries

#### GetAllContactsQuery
**File:** `Features/Contact/Queries/GetAllContactsQuery.cs`

```csharp
public class GetAllContactsQuery : IRequest<GetAllContactsResponse>
{
}

public class GetAllContactsResponse
{
    public required List<ContactDto> Contacts { get; set; }
}

public class ContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MessageCount { get; set; }
}
```

**Handler:** Returns list of all contacts ordered by newest first
**DTOs:** Converts domain enums to strings for view compatibility

---

#### GetContactByIdQuery
**File:** `Features/Contact/Queries/GetContactByIdQuery.cs`

```csharp
public class GetContactByIdQuery : IRequest<GetContactByIdResponse>
{
    public Guid ContactId { get; set; }
}

public class GetContactByIdResponse
{
    public required ContactDetailsDto Contact { get; set; }
}

public class ContactDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public required List<ContactMessageDto> Messages { get; set; }
}

public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string? SenderEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAdminResponse { get; set; }
}
```

**Handler:** Retrieves single contact with all messages, throws KeyNotFoundException if not found
**Messages:** Ordered by creation date, includes admin responses and customer messages

---

### Commands

#### CreateContactCommand
**File:** `Features/Contact/Commands/CreateContactCommand.cs`

```csharp
public class CreateContactCommand : IRequest<CreateContactResponse>
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}

public class CreateContactResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public Guid? ContactId { get; set; }
}
```

**Handler:** Creates new contact via Contact.Create(), persists to repository
**Returns:** Success flag, message, and generated contact ID

---

#### AddContactMessageCommand
**File:** `Features/Contact/Commands/AddContactMessageCommand.cs`

```csharp
public class AddContactMessageCommand : IRequest<AddContactMessageResponse>
{
    public Guid ContactId { get; set; }
    public string Message { get; set; }
    public string? AdminEmail { get; set; }
    public bool IsAdminResponse { get; set; }
}

public class AddContactMessageResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}
```

**Handler:** Adds admin response to contact via AddAdminResponse()
**Status Update:** Automatically sets contact status to Responded

---

#### MarkContactAsReadCommand
**File:** `Features/Contact/Commands/MarkContactAsReadCommand.cs`

```csharp
public class MarkContactAsReadCommand : IRequest<MarkContactAsReadResponse>
{
    public Guid ContactId { get; set; }
}

public class MarkContactAsReadResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}
```

**Handler:** Marks contact as read via MarkAsRead()
**Status Update:** Changes status from New to Read

---

#### UpdateContactStatusCommand
**File:** `Features/Contact/Commands/UpdateContactStatusCommand.cs`

```csharp
public class UpdateContactStatusCommand : IRequest<UpdateContactStatusResponse>
{
    public Guid ContactId { get; set; }
    public string Status { get; set; }
}

public class UpdateContactStatusResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}
```

**Handler:** Updates contact status by parsing enum from string
**Supported Statuses:** Read, Responded, Resolved, Closed
**Methods Called:** MarkAsRead(), Resolve(), Close() based on status

---

## 2. Admin Controller

**File:** `Areas/Admin/Controllers/ContactController.cs`

```csharp
[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminCookie")]
[Route("admin/contacts")]
public class ContactController : Controller
{
    private readonly IMediator _mediator;

    // GET /admin/contacts - List all contacts
    public async Task<IActionResult> Index()
    
    // GET /admin/contacts/create - Show create form
    public IActionResult Create()
    
    // POST /admin/contacts/create - Create new contact
    public async Task<IActionResult> Create(...)
    
    // GET /admin/contacts/{id} - View contact details
    public async Task<IActionResult> Detail(Guid id)
    
    // POST /admin/contacts/{id}/mark-read - Mark as read
    public async Task<IActionResult> MarkAsRead(Guid id)
    
    // POST /admin/contacts/{id}/add-message - Add admin response
    public async Task<IActionResult> AddMessage(Guid id, string message, string adminEmail)
    
    // POST /admin/contacts/{id}/update-status - Update status
    public async Task<IActionResult> UpdateStatus(Guid id, string status)
}
```

**Key Features:**
- All logic delegated to MediatR queries/commands
- No direct repository access
- Error handling with try-catch and KeyNotFoundException
- Anti-forgery tokens on all POST requests
- AdminCookie authentication requirement

---

## 3. Admin Views

### Index View (`Areas/Admin/Views/Contact/Index.cshtml`)

**Features:**
- Table of all contacts with contact count
- Columns: Name, Email, Phone, Subject, Status, Messages, Submitted, Actions
- Status badges with colors:
  - New (red)
  - Read (yellow)
  - Responded (blue)
  - Resolved (green)
  - Closed (gray)
- Message count badge
- View button to detail page
- "New Contact" button to create form
- Empty state message if no contacts

**Data:** Uses ContactDto from GetAllContactsQuery
**Ordering:** Sorted by newest first

---

### Detail View (`Areas/Admin/Views/Contact/Detail.cshtml`)

**Features:**
- Contact information card:
  - Full name, email, phone
  - Subject, status
  - Creation date
  - Mark as Read button (if status is New)

- Message timeline:
  - Customer messages (blue)
  - Admin responses (blue with different badge)
  - Timestamps
  - Sender email
  - Ordered chronologically

- Admin response form:
  - Email input
  - Message textarea
  - Send button
  - Only shows if contact not closed

- Quick actions sidebar:
  - Mark as Resolved button (if not resolved)
  - Close Contact button (if not closed)
  - Confirmation dialog for close action

- Summary card:
  - Total messages count
  - Admin responses count
  - Days open calculation

**Data:** Uses ContactDetailsDto from GetContactByIdQuery

---

### Create View (`Areas/Admin/Views/Contact/Create.cshtml`)

**Features:**
- Form to create new contact:
  - Full name (required)
  - Email (required)
  - Phone number (optional)
  - Subject (required)
  - Message textarea (required)

- Form validation:
  - HTML5 required attributes
  - Email input type validation
  - Tel input type for phone

- Error display:
  - Shows ViewBag.Message for form errors

- Buttons:
  - Cancel button (links to index)
  - Create Contact button (submits form)

- Information sidebar:
  - Lists required fields

**Action:** POSTs to Create handler, redirects to Detail view on success

---

## 4. Admin Routes

```
GET  /admin/contacts           → Index (list all contacts)
GET  /admin/contacts/create    → Create (show form)
POST /admin/contacts/create    → Create (handle form submission)
GET  /admin/contacts/{id}      → Detail (view contact details)
POST /admin/contacts/{id}/mark-read     → MarkAsRead
POST /admin/contacts/{id}/add-message   → AddMessage (with form data)
POST /admin/contacts/{id}/update-status → UpdateStatus (with status param)
```

---

## 5. Data Flow Example

### View Contact List
```
1. Admin visits /admin/contacts
2. ContactController.Index()
3. → _mediator.Send(new GetAllContactsQuery())
4. → GetAllContactsHandler.Handle()
5. → _contactRepository.GetAllAsync()
6. → Maps to List<ContactDto>
7. → View renders Index.cshtml with ContactDto list
```

### Add Admin Response
```
1. Admin submits form on /admin/contacts/{id}
2. ContactController.AddMessage(id, message, adminEmail)
3. → _mediator.Send(AddContactMessageCommand)
4. → AddContactMessageHandler.Handle()
5. → contact.AddAdminResponse(message, email)
6. → _contactRepository.UpdateAsync(contact)
7. → Redirect to Detail view
```

### Update Status
```
1. Admin clicks status button on /admin/contacts/{id}
2. ContactController.UpdateStatus(id, status)
3. → _mediator.Send(UpdateContactStatusCommand)
4. → UpdateContactStatusHandler.Handle()
5. → Enum.TryParse(status)
6. → contact.MarkAsRead() or Resolve() or Close()
7. → _contactRepository.UpdateAsync(contact)
8. → Redirect to Detail view
```

---

## 6. Key Features

✅ **MediatR Pattern**
- All business logic in queries/commands
- Controller only delegates to mediator
- Clean separation of concerns
- Easy to test and extend

✅ **Admin Dashboard**
- List all contacts with pagination-ready
- View details with full conversation history
- Create new contacts manually
- Add admin responses
- Status management

✅ **DTOs for Views**
- ContactDto (minimal data for list)
- ContactDetailsDto (full data for detail)
- ContactMessageDto (message details)
- Type-safe view binding

✅ **Status Management**
- New → Read → Responded → Resolved → Closed
- Quick action buttons
- Enum parsing with validation
- Status-specific UI (hide buttons when closed)

✅ **Authentication**
- AdminCookie required for all routes
- [Authorize] attribute on controller
- Protects all contact management features

✅ **Anti-forgery Protection**
- @Html.AntiForgeryToken() on all forms
- [ValidateAntiForgeryToken] on all POST actions

✅ **Error Handling**
- Try-catch in all handlers
- KeyNotFoundException for missing contacts
- User-friendly error messages
- ModelState validation errors

---

## 7. Summary

The admin contact management system is fully MediatR-based with:
- 2 Queries for retrieving contacts
- 4 Commands for contact operations
- 1 Controller with 7 actions
- 3 Views (Index, Detail, Create)
- DTOs for type-safe view binding
- Full authentication and validation
- Complete CRUD operations through UI
