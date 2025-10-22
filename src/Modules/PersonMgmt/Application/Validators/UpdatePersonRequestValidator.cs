using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

/// <summary>
/// UpdatePersonRequest validator
/// </summary>
public class UpdatePersonRequestValidator : AbstractValidator<UpdatePersonRequest>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public UpdatePersonRequestValidator()
    {
        // E-posta
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin")
            .MaximumLength(100).WithMessage("E-posta maksimum 100 karakter olabilir");

        // Telefon numarası
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Geçerli bir telefon numarası girin")
            .MaximumLength(20).WithMessage("Telefon numarası maksimum 20 karakter olabilir");

        // Profil fotoğrafı URL
        RuleFor(x => x.ProfilePhotoUrl)
            .Must(x => string.IsNullOrEmpty(x) || Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Profil fotoğrafı geçerli bir URL olmalıdır");
    }
}