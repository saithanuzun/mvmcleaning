using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Booking.Entities;

public class Service : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Shortcut { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    public bool IsActive { get; private set; }

    private Service() { }

    public static Service Create(string name, string description, string shortcut, TimeSpan estimatedDuration)
    {
        return new Service
        {
            Name = name,
            Description = description,
            Shortcut = shortcut,
            EstimatedDuration = estimatedDuration,
            IsActive = true
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void UpdateDetails(string name, string description, TimeSpan estimatedDuration)
    {
        Name = name;
        Description = description;
        EstimatedDuration = estimatedDuration;
        UpdatedAt = DateTime.UtcNow;
    }
}