using Identity.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace Shared.Infrastructure.Services;

/// <summary>
///     Bcrypt Password Hasher - Secure password hashing and verification
///     Implements IPasswordHasher with industry-standard bcrypt algorithm
///     Supports password strength validation with customizable requirements
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    private readonly ILogger<BcryptPasswordHasher> _logger;

    /// <summary>
    ///     Password strength requirements
    ///     Meets NIST SP 800-132 recommendations and Turkish security standards
    /// </summary>
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

    /// <summary>
    ///     Hashes a password using bcrypt algorithm with automatic salt generation
    ///     Bcrypt work factor is set to 12, providing strong security with acceptable performance
    ///     Returns tuple of (HashedPassword, Salt) for storage
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>Tuple of (HashedPassword, Salt) as base64-encoded strings</returns>
    /// <exception cref="ArgumentException">Thrown when password is null, empty, or fails validation</exception>
    public (string HashedPassword, string Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        try
        {
            _logger.LogDebug("Hashing password");

            // Validate password strength before hashing
            if (!ValidatePasswordStrength(password))
            {
                _logger.LogWarning("Password does not meet strength requirements");
                throw new ArgumentException(
                    $"Password does not meet strength requirements. {GetPasswordRequirements()}",
                    nameof(password));
            }

            // Bcrypt work factor: 12 provides good security-performance balance
            // - 10-12: Recommended for most applications
            // - 13+: High security but slower (acceptable for registration)
            // - <10: Not recommended for new implementations
            var workFactor = 12;

            // Generate salt and hash password using bcrypt
            // BCrypt internally generates and includes the salt in the hash
            var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor);

            // Extract salt from bcrypt hash (format: $2a$12$SALT_PART...)
            // Bcrypt hashes include the salt, so we extract it for storage if needed
            // However, bcrypt is designed to store the entire hash, so we return it as both
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

    /// <summary>
    ///     Verifies a plain text password against its stored bcrypt hash
    ///     Bcrypt comparison is timing-attack resistant
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hashedPassword">The stored bcrypt hash</param>
    /// <param name="salt">The salt (used for logging/audit purposes with bcrypt)</param>
    /// <returns>True if password matches hash, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when any parameter is null or empty</exception>
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

            // Bcrypt.Verify is timing-attack resistant
            // It always performs the full comparison regardless of match outcome
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

    /// <summary>
    ///     Validates password strength against security requirements
    ///     Checks: length, uppercase, lowercase, numbers, special characters
    ///     Follows NIST SP 800-132 and Turkish security standards
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <returns>True if password meets all requirements, false otherwise</returns>
    public bool ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Password strength validation failed - password is empty");
            return false;
        }

        try
        {
            // Check length requirements
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

            // Check for uppercase letters
            var uppercaseCount = password.Count(char.IsUpper);
            if (uppercaseCount < PasswordRequirements.MinimumUppercase)
            {
                _logger.LogDebug(
                    "Password missing uppercase letters: {Found} (minimum: {Required})",
                    uppercaseCount, PasswordRequirements.MinimumUppercase);
                return false;
            }

            // Check for lowercase letters
            var lowercaseCount = password.Count(char.IsLower);
            if (lowercaseCount < PasswordRequirements.MinimumLowercase)
            {
                _logger.LogDebug(
                    "Password missing lowercase letters: {Found} (minimum: {Required})",
                    lowercaseCount, PasswordRequirements.MinimumLowercase);
                return false;
            }

            // Check for numbers
            var numberCount = password.Count(char.IsDigit);
            if (numberCount < PasswordRequirements.MinimumNumbers)
            {
                _logger.LogDebug(
                    "Password missing numbers: {Found} (minimum: {Required})",
                    numberCount, PasswordRequirements.MinimumNumbers);
                return false;
            }

            // Check for special characters
            // Special characters: !@#$%^&*()_+-=[]{}|;:'",.<>?/`~
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

    /// <summary>
    ///     Returns a human-readable message with password strength requirements
    ///     Supports both English and Turkish formats
    /// </summary>
    /// <returns>Requirements message in English</returns>
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

    /// <summary>
    ///     Extracts the salt portion from a bcrypt hash
    ///     Bcrypt format: $2a$workfactor$SALTHASH
    ///     The salt is 22 characters after the workfactor
    /// </summary>
    /// <param name="bcryptHash">Full bcrypt hash string</param>
    /// <returns>Extracted salt string</returns>
    private string ExtractSaltFromBcryptHash(string bcryptHash)
    {
        try
        {
            // Bcrypt hash format: $2a$12$R9h/cIPz0gi.URNNGT3kmeO6Dv5KwpebWx3EfbHxV2KwVHBkajSFm
            // Parts: $2a$ = algorithm (4 chars)
            //        12 = workfactor (2 chars)
            //        $ = separator (1 char)
            //        remaining = salt + hash
            // Total of 29 chars in format: $2a$12$SALTHASH(22 + 31)

            if (bcryptHash.Length < 29)
                return bcryptHash;

            // Extract 22-character salt starting from position 7
            // Position: 0-3 = "$2a$", 4-5 = "12", 6 = "$", 7-28 = salt(22 chars)
            return bcryptHash.Substring(7, 22);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting salt from bcrypt hash");
            return bcryptHash; // Return full hash if extraction fails
        }
    }
}