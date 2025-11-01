using Identity.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using BCrypt.Net;
namespace Shared.Infrastructure.Services;
public class BcryptPasswordHasher : IPasswordHasher
{
    private readonly ILogger<BcryptPasswordHasher> _logger;
    private static class PasswordRequirements
    {
        public const int MinimumLength = 8;
        public const int MaximumLength = 128;
        public const int MinimumUppercase = 1;
        public const int MinimumLowercase = 1;
        public const int MinimumNumbers = 1;
        public const int MinimumSpecialChars = 1;
    }
    public BcryptPasswordHasher(ILogger<BcryptPasswordHasher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("BcryptPasswordHasher initialized");
    }
    public (string HashedPassword, string Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        try
        {
            _logger.LogDebug("Hashing password");
            if (!ValidatePasswordStrength(password))
            {
                _logger.LogWarning("Password does not meet strength requirements");
                throw new ArgumentException(
                    $"Password does not meet strength requirements. {GetPasswordRequirements()}",
                    nameof(password));
            }
            var workFactor = 12;
            var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor);
            var salt = ExtractSaltFromBcryptHash(hash);
            _logger.LogInformation("Password hashed successfully using bcrypt (work factor: {WorkFactor})",
                workFactor);
            return (hash, salt);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw new InvalidOperationException("An error occurred while hashing the password", ex);
        }
    }
    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("VerifyPassword called with null or empty password");
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            _logger.LogWarning("VerifyPassword called with null or empty hash");
            throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));
        }
        try
        {
            _logger.LogDebug("Verifying password against hash");
            var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            if (isValid)
            {
                _logger.LogDebug("Password verification successful");
                return true;
            }
            _logger.LogWarning("Password verification failed - hash mismatch");
            return false;
        }
        catch (SaltParseException ex)
        {
            _logger.LogError(ex, "Invalid bcrypt hash format");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password verification");
            return false;
        }
    }
    public bool ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Password strength validation failed - password is empty");
            return false;
        }
        try
        {
            if (password.Length < PasswordRequirements.MinimumLength)
            {
                _logger.LogDebug(
                    "Password too short: {Length} characters (minimum: {Minimum})",
                    password.Length, PasswordRequirements.MinimumLength);
                return false;
            }
            if (password.Length > PasswordRequirements.MaximumLength)
            {
                _logger.LogDebug(
                    "Password too long: {Length} characters (maximum: {Maximum})",
                    password.Length, PasswordRequirements.MaximumLength);
                return false;
            }
            var uppercaseCount = password.Count(char.IsUpper);
            if (uppercaseCount < PasswordRequirements.MinimumUppercase)
            {
                _logger.LogDebug(
                    "Password missing uppercase letters: {Found} (minimum: {Required})",
                    uppercaseCount, PasswordRequirements.MinimumUppercase);
                return false;
            }
            var lowercaseCount = password.Count(char.IsLower);
            if (lowercaseCount < PasswordRequirements.MinimumLowercase)
            {
                _logger.LogDebug(
                    "Password missing lowercase letters: {Found} (minimum: {Required})",
                    lowercaseCount, PasswordRequirements.MinimumLowercase);
                return false;
            }
            var numberCount = password.Count(char.IsDigit);
            if (numberCount < PasswordRequirements.MinimumNumbers)
            {
                _logger.LogDebug(
                    "Password missing numbers: {Found} (minimum: {Required})",
                    numberCount, PasswordRequirements.MinimumNumbers);
                return false;
            }
            var specialCharPattern = @"[!@#$%^&*()_+\-=\[\]{}|;:'"",.<>?/`~\\]";
            var specialCharCount = Regex.Matches(password, specialCharPattern).Count;
            if (specialCharCount < PasswordRequirements.MinimumSpecialChars)
            {
                _logger.LogDebug(
                    "Password missing special characters: {Found} (minimum: {Required})",
                    specialCharCount, PasswordRequirements.MinimumSpecialChars);
                return false;
            }
            _logger.LogDebug("Password passed all strength validations");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password strength");
            return false;
        }
    }
    public string GetPasswordRequirements()
    {
        var requirements = new[]
        {
            $"Minimum length: {PasswordRequirements.MinimumLength} characters",
            $"Maximum length: {PasswordRequirements.MaximumLength} characters",
            $"Minimum uppercase letters: {PasswordRequirements.MinimumUppercase}",
            $"Minimum lowercase letters: {PasswordRequirements.MinimumLowercase}",
            $"Minimum numbers: {PasswordRequirements.MinimumNumbers}",
            $"Minimum special characters: {PasswordRequirements.MinimumSpecialChars} (!@#$%^&*...)"
        };
        return "Password must meet the following requirements:\n" + string.Join("\n", requirements);
    }
    private string ExtractSaltFromBcryptHash(string bcryptHash)
    {
        try
        {
            if (bcryptHash.Length < 29)
                return bcryptHash;
            return bcryptHash.Substring(7, 22);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting salt from bcrypt hash");
            return bcryptHash;
        }
    }
}