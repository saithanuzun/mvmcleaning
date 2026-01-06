using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.SharedKernel.ValueObjects;

public class TimeSlot : ValueObject
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan Duration => EndTime - StartTime;

    private TimeSlot() { }

    public static TimeSlot Create(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        return new TimeSlot { StartTime = startTime, EndTime = endTime };
    }

    public bool OverlapsWith(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }
}