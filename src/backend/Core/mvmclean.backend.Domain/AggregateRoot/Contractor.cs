using mvmclean.backend.Domain.Entities;
using mvmclean.backend.Domain.ValueObjects;
using WorkingHours = mvmclean.backend.Domain.ValueObjects.WorkingHours;

namespace mvmclean.backend.Domain.AggregateRoot;

public class Contractor : Common.AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }
    private readonly List<Review> _reviews = new();

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private readonly List<WorkingHours> _workingHours = new();
    public IReadOnlyCollection<WorkingHours> WorkingHours => _workingHours.AsReadOnly();

    private readonly List<ContractorCoverage> _coverageAreas = new();
    public IReadOnlyCollection<ContractorCoverage> CoverageAreas => _coverageAreas.AsReadOnly();
    
    private readonly List<TimeSlot> _unavailableSlots = new();
    public IReadOnlyCollection<TimeSlot> UnavailableSlots => _unavailableSlots.AsReadOnly();

    private Contractor()
    {
    }

    public static Contractor Create(string firstName, string lastName, PhoneNumber phoneNumber, string email)
    {
        return new Contractor
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = email,
            IsActive = true
        };
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
        if (!_coverageAreas.Any(c => c.Postcode.Equals(postcode) && c.IsActive))
        {
            _coverageAreas.Add(ContractorCoverage.Create(Id, postcode));
        }
    }

    public void RemoveCoverageArea(Postcode postcode)
    {
        var coverage = _coverageAreas.FirstOrDefault(c => c.Postcode.Equals(postcode));
        coverage?.Deactivate();
    }

    public void MarkAsUnavailable(TimeSlot timeSlot)
    {
        if (_unavailableSlots.Any(s => s.OverlapsWith(timeSlot)))
        {
            throw new InvalidOperationException("Time slot overlaps with existing unavailability");
        }

        _unavailableSlots.Add(timeSlot);
    }

    public bool IsAvailableAt(TimeSlot timeSlot, Postcode postcode)
    {
        if (!IsActive) return false;
        if (!CoversPostcode(postcode)) return false;
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

    public string FullName => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();
    
    public void AddReview(Review review)
    {
        _reviews.Add(review);
    }

}
