using FluentValidation;

namespace Identity.Application.Validators;

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