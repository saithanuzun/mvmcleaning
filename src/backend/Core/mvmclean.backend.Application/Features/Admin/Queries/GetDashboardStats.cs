using MediatR;
using mvmclean.backend.Domain.Aggregates.Booking;
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
    public List<RecentContractorDto> RecentContractors { get; set; } = new();
    public List<RecentServiceDto> RecentServices { get; set; } = new();
    public List<RecentBookingDto> RecentBookings { get; set; } = new();
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

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsRequest, GetDashboardStatsResponse>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetDashboardStatsHandler(
        IContractorRepository contractorRepository,
        IServiceRepository serviceRepository,
        IBookingRepository bookingRepository)
    {
        _contractorRepository = contractorRepository;
        _serviceRepository = serviceRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<GetDashboardStatsResponse> Handle(GetDashboardStatsRequest request, CancellationToken cancellationToken)
    {
        var contractors = await _contractorRepository.GetAll();
        var services = await _serviceRepository.GetAll();
        var bookings = await _bookingRepository.GetAll();

        var response = new GetDashboardStatsResponse
        {
            TotalContractors = contractors.Count,
            ActiveContractors = contractors.Count(c => c.IsActive),
            TotalServices = services.Count,
            TotalBookings = bookings.Count,
            PendingBookings = bookings.Count(b => b.Status.ToString() == "Pending"),
            ConfirmedBookings = bookings.Count(b => b.Status.ToString() == "Confirmed"),
            CompletedBookings = bookings.Count(b => b.Status.ToString() == "Completed"),

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
                .ToList()
        };

        return response;
    }
}
