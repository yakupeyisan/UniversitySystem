using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

public class UpdatePersonRequestValidator : AbstractValidator<UpdatePersonRequest>
{
    public UpdatePersonRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("E-posta boş olamaz")
            .NotEmpty().WithMessage("E-posta boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin")
            .MaximumLength(100).WithMessage("E-posta maksimum 100 karakter olabilir");
        RuleFor(x => x.PhoneNumber)
            .NotNull().WithMessage("Telefon numarası boş olamaz")
            .NotEmpty().WithMessage("Telefon numarası boş olamaz")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Geçerli bir telefon numarası girin")
            .MaximumLength(20).WithMessage("Telefon numarası maksimum 20 karakter olabilir");
        RuleFor(x => x.ProfilePhotoUrl)
            .Must(x => string.IsNullOrEmpty(x) || Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Profil fotoğrafı geçerli bir URL olmalıdır");
    }
}