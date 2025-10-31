using FluentValidation;
using Identity.Application.Commands;

namespace Identity.Application.Validators;

public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Request).NotNull().WithMessage("Assign role request cannot be null");

        RuleFor(x => x.Request.RoleId)
            .NotEmpty().WithMessage("Role ID is required");
    }
}

public class PasswordStrengthValidator : AbstractValidator<string>
{
    public PasswordStrengthValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Parola boþ olamaz")
            .MinimumLength(8).WithMessage("Parola en az 8 karakter olmalý")
            .Matches("[A-Z]").WithMessage("Parola en az 1 büyük harf içermeli")
            .Matches("[a-z]").WithMessage("Parola en az 1 küçük harf içermeli")
            .Matches("[0-9]").WithMessage("Parola en az 1 rakam içermeli")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]")
            .WithMessage("Parola en az 1 özel karakter içermeli");
    }
}

public class TwoFactorCodeValidator : AbstractValidator<string>
{
    public TwoFactorCodeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("2FA kodu boþ olamaz")
            .Length(6).WithMessage("2FA kodu 6 rakam olmalý")
            .Matches("^[0-9]{6}$").WithMessage("2FA kodu sadece rakam içermeli");
    }
}

public class BackupCodeValidator : AbstractValidator<string>
{
    public BackupCodeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Backup kodu boþ olamaz")
            .Length(8).WithMessage("Backup kodu 8 karakter olmalý")
            .Matches("^[A-F0-9]{8}$").WithMessage("Backup kodu A-F ve 0-9 içermeli");
    }
}