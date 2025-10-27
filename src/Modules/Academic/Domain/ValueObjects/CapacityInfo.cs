using Core.Domain.ValueObjects;
namespace Academic.Domain.ValueObjects;
public class CapacityInfo : ValueObject
{
    public int MaxCapacity { get; }
    public int CurrentEnrollment { get; }
    private CapacityInfo(int maxCapacity, int currentEnrollment = 0)
    {
        MaxCapacity = maxCapacity;
        CurrentEnrollment = currentEnrollment;
    }
    public static CapacityInfo Create(int maxCapacity)
    {
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");
        if (maxCapacity > 500)
            throw new ArgumentException("Max capacity cannot exceed 500");
        return new CapacityInfo(maxCapacity, 0);
    }
    public bool IsFull() => CurrentEnrollment >= MaxCapacity;
    public bool HasAvailableSeats() => CurrentEnrollment < MaxCapacity;
    public int AvailableSeats() => Math.Max(0, MaxCapacity - CurrentEnrollment);
    public float OccupancyPercentage() => (float)CurrentEnrollment / MaxCapacity * 100;
    public CapacityInfo WithIncrementedEnrollment()
    {
        if (IsFull())
            throw new InvalidOperationException("Cannot increment enrollment when capacity is full");
        return new CapacityInfo(MaxCapacity, CurrentEnrollment + 1);
    }
    public CapacityInfo WithDecrementedEnrollment()
    {
        if (CurrentEnrollment <= 0)
            throw new InvalidOperationException("Cannot decrement enrollment below 0");
        return new CapacityInfo(MaxCapacity, CurrentEnrollment - 1);
    }
    public CapacityInfo WithUpdatedCapacity(int newMaxCapacity)
    {
        if (newMaxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");
        if (newMaxCapacity < CurrentEnrollment)
            throw new ArgumentException("New capacity cannot be less than current enrollment");
        if (newMaxCapacity > 500)
            throw new ArgumentException("Max capacity cannot exceed 500");
        return new CapacityInfo(newMaxCapacity, CurrentEnrollment);
    }
    public override string ToString() => $"{CurrentEnrollment}/{MaxCapacity}";
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MaxCapacity;
        yield return CurrentEnrollment;
    }
}