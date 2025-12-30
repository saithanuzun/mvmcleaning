using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.ValueObjects;

public class WorkingHours : ValueObject
{
    public DayOfWeek DayOfWeek { get; }
    public bool IsWorkingDay { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    private WorkingHours() { }

    private WorkingHours(
        DayOfWeek dayOfWeek,
        bool isWorkingDay,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (isWorkingDay && startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        DayOfWeek = dayOfWeek;
        IsWorkingDay = isWorkingDay;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static WorkingHours CreateWorkingDay(
        DayOfWeek dayOfWeek,
        TimeOnly start,
        TimeOnly end)
        => new(dayOfWeek, true, start, end);

    public static WorkingHours CreateDayOff(DayOfWeek dayOfWeek)
        => new(dayOfWeek, false, default, default);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DayOfWeek;
        yield return IsWorkingDay;
        yield return StartTime;
        yield return EndTime;
    }
}
