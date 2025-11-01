using FluentValidation;
namespace Identity.Application.Validators;
public class PasswordStrengthValidator : AbstractValidator<string>
{
    public PasswordStrengthValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Parola bo� olamaz")
            .MinimumLength(8).WithMessage("Parola en az 8 karakter olmal�")
            .Matches("[A-Z]").WithMessage("Parola en az 1 b�y�k harf i�ermeli")
            .Matches("[a-z]").WithMessage("Parola en az 1 k���k harf i�ermeli")
            .Matches("[0-9]").WithMessage("Parola en az 1 rakam i�ermeli")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]")
            .WithMessage("Parola en az 1 �zel karakter i�ermeli");
    }
}