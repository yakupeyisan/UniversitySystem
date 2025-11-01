using Core.Domain.ValueObjects;
namespace Identity.Domain.ValueObjects;
public class PasswordHash : ValueObject
{
    public PasswordHash(string hashedPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPassword));
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));
        HashedPassword = hashedPassword;
        Salt = salt;
    }
    public PasswordHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPassword));
        HashedPassword = hashedPassword;
        Salt = string.Empty;
    }
    public string HashedPassword { get; }
    public string Salt { get; }
    public override string ToString()
    {
        return HashedPassword;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedPassword;
        yield return Salt;
    }
}