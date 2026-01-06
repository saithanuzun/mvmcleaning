using mvmclean.backend.Domain.Aggregates.Booking.ValueObjects;
using mvmclean.backend.Domain.Aggregates.Contractor.Entities;
using mvmclean.backend.Domain.Aggregates.Contractor.Events;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;
using WorkingHours = mvmclean.backend.Domain.Aggregates.Contractor.Entities.WorkingHours;

namespace mvmclean.backend.Domain.Aggregates.Contractor;

public class Contractor : Core.BaseClasses.AggregateRoot
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? ImageUrl { get; set; }
    
    public string Username { get; private set; } 
    public string PasswordHash { get; private set; }
    
    public int BookedCount { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    
    private readonly List<Guid> _services = new();
    public IReadOnlyCollection<Guid> Services => _services.AsReadOnly();

    private readonly List<WorkingHours> _workingHours = new();
    public IReadOnlyCollection<WorkingHours> WorkingHours => _workingHours.AsReadOnly();

    private readonly List<ContractorCoverage> _coverageAreas = new();
    public IReadOnlyCollection<ContractorCoverage> CoverageAreas => _coverageAreas.AsReadOnly();
    
    private readonly List<TimeSlot> _unavailableSlots = new();
    public IReadOnlyCollection<TimeSlot> UnavailableSlots => _unavailableSlots.AsReadOnly();

    private Contractor() { }

    public static Contractor Create(
        string firstName, 
        string lastName, 
        string phoneNumber, 
        string email, 
        string? imageUrl,
        string username,
        string password)
    {
        var contractor = new Contractor
        {
            Id = Guid.NewGuid(), 
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = PhoneNumber.Create(phoneNumber),
            Email = Email.Create(email),
            IsActive = false,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            Username = username,
            PasswordHash = password,
            BookedCount = 0,
        };

        contractor.AddDomainEvent(new ContractorCreatedEvent(contractor.Id.ToString(), contractor.FullName, contractor.Email.ToString()));

        return contractor;
    }


    public void AddWorkingHours(WorkingHours workingHours)
    {
        var existing = _workingHours.FirstOrDefault(w => w.DayOfWeek == workingHours.DayOfWeek);
        if (existing != null)
        {
            _workingHours.Remove(existing);
        }

        _workingHours.Add(workingHours);
    }

    public void AddCoverageArea(Postcode postcode)
    {
        if (_coverageAreas.All(c => c.Postcode.Area != postcode.Area))
        {
            _coverageAreas.Add(ContractorCoverage.Create(Id, postcode));
        }
    }
    

    public void AddService(Guid serviceId)
    {
        _services.Add(serviceId);
        
    }
    public void RemoveService(Guid serviceId)
    {
        _services.Remove(serviceId);

    }

    public void RemoveCoverageArea(Postcode postcode)
    {
        var coverage = _coverageAreas.FirstOrDefault(c => c.Postcode.Area == postcode.Area);
        coverage?.Deactivate();
    }

    public void MarkAsUnavailable(TimeSlot timeSlot)
    {
        _unavailableSlots.Add(timeSlot);
    }

    public bool IsAvailableAt(TimeSlot timeSlot)
    {
        if (!IsActive) return false;
        if (_unavailableSlots.Any(s => s.OverlapsWith(timeSlot))) return false;

        var dayOfWeek = (DayOfWeek)((int)timeSlot.StartTime.DayOfWeek == 0 ? 7 : (int)timeSlot.StartTime.DayOfWeek);
        var workingHours = _workingHours.FirstOrDefault(w => w.DayOfWeek == dayOfWeek);

        if (workingHours == null || !workingHours.IsWorkingDay) return false;

        var slotStart = TimeOnly.FromDateTime(timeSlot.StartTime);
        var slotEnd = TimeOnly.FromDateTime(timeSlot.EndTime);

        return slotStart >= workingHours.StartTime && slotEnd <= workingHours.EndTime;
    }

    public bool CoversPostcode(Postcode postcode)
    {
        return _coverageAreas.Any(c => c.IsActive &&
                                       (c.Postcode.Value == postcode.Value ||
                                        c.Postcode.Area == postcode.Area ||
                                        c.Postcode.District == postcode.District));
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetImage(string imageUrl) => ImageUrl = imageUrl;

    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();
    
    public void AddReview(Review review)
    {
        _reviews.Add(review);
    }

    public void IncreaseBookedCount()
    {
        BookedCount++;
    }

}
