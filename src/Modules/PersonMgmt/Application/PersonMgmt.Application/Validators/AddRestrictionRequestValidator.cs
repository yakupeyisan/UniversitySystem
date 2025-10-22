using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

/// <summary>
/// AddRestrictionRequest validator
/// </summary>
public class AddRestrictionRequestValidator : AbstractValidator<AddRestrictionRequest>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AddRestrictionRequestValidator()
    {
        // Kısıtlama türü
        RuleFor(x => x.RestrictionType)
            .InclusiveBetween((byte)0, (byte)2).WithMessage("Kısıtlama türü geçerli olmalıdır");

        // Kısıtlama seviyesi
        RuleFor(x => x.RestrictionLevel)
            .InclusiveBetween((byte)0, (byte)2).WithMessage("Kısıtlama seviyesi geçerli olmalıdır");

        // Başlangıç tarihi
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Başlangıç tarihi bugünden önce olmalıdır");

        // Bitiş tarihi
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır")
            .When(x => x.EndDate.HasValue);

        // Neden
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Neden boş olamaz")
            .MaximumLength(500).WithMessage("Neden maksimum 500 karakter olabilir");

        // Şiddeti
        RuleFor(x => x.Severity)
            .InclusiveBetween(1, 10).WithMessage("Şiddeti 1 ile 10 arasında olmalıdır");
    }
}