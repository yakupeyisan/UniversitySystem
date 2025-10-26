using Core.Domain.ValueObjects;

namespace Academic.Domain.ValueObjects;

/// <summary>
/// ValueObject representing a time slot with start and end times
/// </summary>
public class TimeSlot : ValueObject
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public int DurationMinutes { get; }

    private TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
        DurationMinutes = CalculateDuration(startTime, endTime);
    }

    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        return new TimeSlot(startTime, endTime);
    }

    private static int CalculateDuration(TimeOnly startTime, TimeOnly endTime)
    {
        var span = endTime.ToTimeSpan() - startTime.ToTimeSpan();
        return (int)span.TotalMinutes;
    }

    public bool ConflictsWith(TimeSlot other)
    {
        return !(EndTime <= other.StartTime || StartTime >= other.EndTime);
    }

    public override string ToString() => $"{StartTime:HH:mm} - {EndTime:HH:mm}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }
}