using System.Text.RegularExpressions;
using Core.Domain.ValueObjects;
namespace Identity.Domain.ValueObjects;
public class Email : ValueObject
{
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));
        value = value.Trim().ToLowerInvariant();
        if (!IsValidEmail(value))
            throw new ArgumentException("Email format is invalid", nameof(value));
        Value = value;
    }
    public string Value { get; }
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
    public static implicit operator string(Email email)
    {
        return email.Value;
    }
    public static implicit operator Email(string value)
    {
        return new Email(value);
    }
    public override string ToString()
    {
        return Value;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}