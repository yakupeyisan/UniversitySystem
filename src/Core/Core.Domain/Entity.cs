namespace Core.Domain;

/// <summary>
/// Entity - Tüm domain entities'in base class'ı
/// 
/// Özellikleri:
/// - Identity (Id) - Sadece bu sayede bir entity diğerinden ayırt edilir
/// - Equality - Id'ye göre eşitlik kontrolü
/// - Value Equality (structural equality)
/// 
/// Not: Entity'ler mutable'dır ve identity'leri değişmez
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>
    /// Entity'nin unique identifier'ı
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Optimistic concurrency control için
    /// EF Core tarafından otomatik olarak yönetilir
    /// </summary>
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

    /// <summary>
    /// Entity'ler sadece Id'ye göre karşılaştırılır
    /// </summary>
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

    /// <summary>
    /// Hash code hesapla
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Entity'ler arasında eşitlik kontrolü
    /// </summary>
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Entity'ler arasında eşitsizlik kontrolü
    /// </summary>
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}