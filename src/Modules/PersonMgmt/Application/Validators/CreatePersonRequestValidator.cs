using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

/// <summary>
/// CreatePersonRequest validator
/// </summary>
public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public CreatePersonRequestValidator()
    {
        // Ad
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz")
            .MaximumLength(50).WithMessage("Ad maksimum 50 karakter olabilir");

        // Soyad
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz")
            .MaximumLength(50).WithMessage("Soyad maksimum 50 karakter olabilir");

        // T.C. Kimlik Numarası
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("T.C. Kimlik Numarası boş olamaz")
            .Length(11).WithMessage("T.C. Kimlik Numarası 11 karakter olmalıdır")
            .Matches(@"^\d+$").WithMessage("T.C. Kimlik Numarası sadece rakam içermelidir");

        // Doğum tarihi
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz")
            .LessThan(DateTime.UtcNow).WithMessage("Doğum tarihi bugünden önce olmalıdır")
            .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Geçerli bir doğum tarihi girin");

        // Cinsiyet
        RuleFor(x => x.Gender)
            .InclusiveBetween((byte)0, (byte)1).WithMessage("Cinsiyet 0 (Erkek) veya 1 (Kadın) olmalıdır");

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