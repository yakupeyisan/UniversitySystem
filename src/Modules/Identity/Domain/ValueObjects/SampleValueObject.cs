using Core.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Identity.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        value = value.Trim().ToLowerInvariant();

        if (!IsValidEmail(value))
            throw new ArgumentException("Email format is invalid", nameof(value));

        Value = value;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);

    public override string ToString() => Value;
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
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


