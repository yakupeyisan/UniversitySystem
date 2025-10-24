namespace Core.Domain;
public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    public byte[]? RowVersion { get; set; }
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));
        Id = id;
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
            return false;
        return Id == entity.Id && GetType() == entity.GetType();
    }
    public bool Equals(Entity? other)
    {
        return Equals((object?)other);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}