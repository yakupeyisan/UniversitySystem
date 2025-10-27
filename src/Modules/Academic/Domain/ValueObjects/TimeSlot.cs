using Core.Domain.ValueObjects;
namespace Academic.Domain.ValueObjects;
public class TimeSlot : ValueObject
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public int DurationMinutes { get; }
    private TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
        DurationMinutes = (int)(endTime - startTime).TotalMinutes;
    }
    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        var duration = (int)(endTime - startTime).TotalMinutes;
        if (duration < 30)
            throw new ArgumentException("Time slot duration must be at least 30 minutes");
        if (duration > 480)
            throw new ArgumentException("Time slot duration cannot exceed 8 hours");
        return new TimeSlot(startTime, endTime);
    }
    public static TimeSlot Create(string startTimeString, string endTimeString)
    {
        if (!TimeOnly.TryParse(startTimeString, out var startTime))
            throw new ArgumentException("Invalid start time format. Use HH:mm");
        if (!TimeOnly.TryParse(endTimeString, out var endTime))
            throw new ArgumentException("Invalid end time format. Use HH:mm");
        return Create(startTime, endTime);
    }
    public bool OverlapsWith(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
    public bool Contains(TimeOnly time)
    {
        return time >= StartTime && time < EndTime;
    }
    public override string ToString() => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }
}