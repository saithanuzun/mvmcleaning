using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Contractor.Entities;

public class WorkingHours : Entity
{
    
    public DayOfWeek DayOfWeek { get; private set; }
    public bool IsWorkingDay { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public Contractor Contractor { get; set; }
    public Guid ContractorId { get; set; }

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

}
