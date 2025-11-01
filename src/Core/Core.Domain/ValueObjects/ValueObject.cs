namespace Core.Domain.ValueObjects;
public abstract class ValueObject : IEquatable<ValueObject>
{
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }
    protected abstract IEnumerable<object?> GetEqualityComponents();
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;
        var valueObject = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}