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