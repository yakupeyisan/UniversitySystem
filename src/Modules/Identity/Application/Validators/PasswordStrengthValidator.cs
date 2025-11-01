using FluentValidation;

namespace Identity.Application.Validators;

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