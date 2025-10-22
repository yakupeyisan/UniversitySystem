namespace Core.Domain.ValueObjects;

/// <summary>
/// ValueObject - Değeri önemli olan, identity'si olmayan object
/// 
/// Özellikleri:
/// - Immutable (değişmez)
/// - No identity (sadece değerleri önemli)
/// - Value equality (tüm property'lerine göre karşılaştırılır)
/// - Attribute-focused, not identity-focused
/// 
/// Örnekler:
/// - Money (10 USD)
/// - Address (Street, City, Country)
/// - Email (john@example.com)
/// - PersonId (Value Object olarak tanımlanabilir)
/// 
/// Entity vs ValueObject:
/// Entity: Person (Id, Name, Email, ...) - identity önemli
/// ValueObject: Address (Street, City, Country) - value önemli
/// 
/// İki Address'in eşit olması:
/// var addr1 = new Address("Main St", "NYC", "USA");
/// var addr2 = new Address("Main St", "NYC", "USA");
/// addr1 == addr2 → true (Entity'lerde false olurdu!)
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Tüm component'lerin value'larını getir
    /// equality ve hash code'u hesaplamak için
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Equality - tüm component'lere göre
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var valueObject = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    /// <summary>
    /// Hash code - tüm component'lere göre hesapla
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// ValueObjects arasında eşitlik
    /// </summary>
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

/// <summary>
/// Örnek: PersonId ValueObject
/// 
/// public class PersonId : ValueObject
/// {
///     public Guid Value { get; }
///
///     public PersonId(Guid value)
///     {
///         if (value == Guid.Empty)
///             throw new ArgumentException("PersonId cannot be empty");
///         Value = value;
///     }
///
///     protected override IEnumerable<object?> GetEqualityComponents()
///     {
///         yield return Value;
///     }
///
///     public override string ToString() => Value.ToString();
/// }
/// 
/// Kullanım:
/// var id1 = new PersonId(Guid.Parse("..."));
/// var id2 = new PersonId(Guid.Parse("..."));
/// id1 == id2 → true (aynı value'ya sahipse)
/// </summary>