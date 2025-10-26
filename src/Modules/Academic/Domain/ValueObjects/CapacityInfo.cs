using Core.Domain.ValueObjects;

namespace Academic.Domain.ValueObjects;

/// <summary>
/// ValueObject representing capacity information for a course
/// </summary>
public class CapacityInfo : ValueObject
{
    public int MaxCapacity { get; }
    public int CurrentEnrollment { get; }
    public int AvailableSeats { get; }
    public decimal OccupancyPercentage { get; }

    private CapacityInfo(int maxCapacity, int currentEnrollment)
    {
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");

        if (currentEnrollment < 0)
            throw new ArgumentException("Current enrollment cannot be negative");

        if (currentEnrollment > maxCapacity)
            throw new ArgumentException("Current enrollment cannot exceed max capacity");

        MaxCapacity = maxCapacity;
        CurrentEnrollment = currentEnrollment;
        AvailableSeats = maxCapacity - currentEnrollment;
        OccupancyPercentage = (currentEnrollment / (decimal)maxCapacity) * 100;
    }

    public static CapacityInfo Create(int maxCapacity, int currentEnrollment = 0)
    {
        return new CapacityInfo(maxCapacity, currentEnrollment);
    }

    public bool IsFull() => CurrentEnrollment >= MaxCapacity;

    public bool HasAvailableSeats() => AvailableSeats > 0;

    public CapacityInfo WithIncrementedEnrollment()
    {
        if (IsFull())
            throw new InvalidOperationException("Cannot increment enrollment - capacity is full");

        return new CapacityInfo(MaxCapacity, CurrentEnrollment + 1);
    }

    public CapacityInfo WithDecrementedEnrollment()
    {
        if (CurrentEnrollment <= 0)
            throw new InvalidOperationException("Cannot decrement enrollment below 0");

        return new CapacityInfo(MaxCapacity, CurrentEnrollment - 1);
    }

    public override string ToString() => $"{CurrentEnrollment}/{MaxCapacity} ({OccupancyPercentage:F1}%)";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MaxCapacity;
        yield return CurrentEnrollment;
    }
}