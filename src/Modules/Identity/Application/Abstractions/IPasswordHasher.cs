namespace Identity.Application.Abstractions;

/// <summary>
/// Password hashing and verification service interface
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using secure algorithm
    /// Returns tuple of (HashedPassword, Salt)
    /// </summary>
    (string HashedPassword, string Salt) HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword, string salt);

    /// <summary>
    /// Validates password strength
    /// </summary>
    bool ValidatePasswordStrength(string password);

    /// <summary>
    /// Gets password strength requirements message
    /// </summary>
    string GetPasswordRequirements();
}