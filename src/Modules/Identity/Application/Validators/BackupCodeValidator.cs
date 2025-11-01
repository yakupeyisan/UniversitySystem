using FluentValidation;
namespace Identity.Application.Validators;
public class BackupCodeValidator : AbstractValidator<string>
{
    public BackupCodeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Backup kodu bo� olamaz")
            .Length(8).WithMessage("Backup kodu 8 karakter olmal�")
            .Matches("^[A-F0-9]{8}$").WithMessage("Backup kodu A-F ve 0-9 i�ermeli");
    }
}