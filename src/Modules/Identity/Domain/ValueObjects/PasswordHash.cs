using Core.Domain.ValueObjects;

namespace Identity.Domain.ValueObjects;

public class PasswordHash : ValueObject
{
    public string HashedPassword { get; }
    public string Salt { get; }

    public PasswordHash(string hashedPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPassword));

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));

        HashedPassword = hashedPassword;
        Salt = salt;
    }

    public override string ToString() => HashedPassword;
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedPassword;
        yield return Salt;
    }
}