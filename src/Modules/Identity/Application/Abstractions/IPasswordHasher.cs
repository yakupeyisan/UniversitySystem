namespace Identity.Application.Abstractions;
public interface IPasswordHasher
{
    (string HashedPassword, string Salt) HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword, string salt);
    bool ValidatePasswordStrength(string password);
    string GetPasswordRequirements();
}