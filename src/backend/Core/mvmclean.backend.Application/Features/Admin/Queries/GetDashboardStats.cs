using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
using mvmclean.backend.Domain.Aggregates.Contact;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Service;

namespace mvmclean.backend.Application.Features.Admin.Queries;

public class GetDashboardStatsRequest : IRequest<GetDashboardStatsResponse>
{
}

public class GetDashboardStatsResponse
{
    public int TotalContractors { get; set; }
    public int ActiveContractors { get; set; }
    public int TotalServices { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int TotalContacts { get; set; }
    public int NewContacts { get; set; }
    public int RespondedContacts { get; set; }
    public List<RecentContractorDto> RecentContractors { get; set; } = new();
    public List<RecentServiceDto> RecentServices { get; set; } = new();
    public List<RecentBookingDto> RecentBookings { get; set; } = new();
    public List<RecentContactDto> RecentContacts { get; set; } = new();
}

public class RecentContractorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentBookingDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Status { get; set; }
    public int MessageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsRequest, GetDashboardStatsResponse>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IContactRepository _contactRepository;

    public GetDashboardStatsHandler(
        IContractorRepository contractorRepository,
        IServiceRepository serviceRepository,
        IBookingRepository bookingRepository,
        IContactRepository contactRepository)
    {
        _contractorRepository = contractorRepository;
        _serviceRepository = serviceRepository;
        _bookingRepository = bookingRepository;
        _contactRepository = contactRepository;
    }

    public async Task<GetDashboardStatsResponse> Handle(GetDashboardStatsRequest request, CancellationToken cancellationToken)
    {
        var contractors = await _contractorRepository.GetAll();
        var services = await _serviceRepository.GetAll();
        var bookings = await _bookingRepository.GetAll();
        var contacts = await _contactRepository.GetAll();

        var response = new GetDashboardStatsResponse
        {
            TotalContractors = contractors.Count,
            ActiveContractors = contractors.Count(c => c.IsActive),
            TotalServices = services.Count,
            TotalBookings = bookings.Count,
            PendingBookings = bookings.Count(b => b.Status.ToString() == "Pending"),
            ConfirmedBookings = bookings.Count(b => b.Status.ToString() == "Confirmed"),
            CompletedBookings = bookings.Count(b => b.Status.ToString() == "Completed"),
            TotalContacts = contacts.Count,
            NewContacts = contacts.Count(c => c.Status.ToString() == "New"),
            RespondedContacts = contacts.Count(c => c.Status.ToString() == "Responded"),

            RecentContractors = contractors
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new RecentContractorDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToList(),

            RecentServices = services
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .Select(s => new RecentServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category ?? "Uncategorized",
                    BasePrice = s.BasePrice.Amount,
                    CreatedAt = s.CreatedAt
                })
                .ToList(),

            RecentBookings = bookings
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new RecentBookingDto
                {
                    Id = b.Id,
                    CustomerName = b.Customer?.FirstName ?? "No Customer Assigned",
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                })
                .ToList(),

            RecentContacts = contacts
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new RecentContactDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    Subject = c.Subject,
                    Status = c.Status.ToString(),
                    MessageCount = c.Messages.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };

        return response;
    }
}
