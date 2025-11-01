using FluentValidation;

namespace Identity.Application.Validators;

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