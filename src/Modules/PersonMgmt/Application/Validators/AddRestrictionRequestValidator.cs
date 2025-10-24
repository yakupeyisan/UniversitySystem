using FluentValidation;
using PersonMgmt.Application.DTOs;
namespace PersonMgmt.Application.Validators;
public class AddRestrictionRequestValidator : AbstractValidator<AddRestrictionRequest>
{
    public AddRestrictionRequestValidator()
    {
        RuleFor(x => x.RestrictionType)
            .InclusiveBetween((byte)0, (byte)2).WithMessage("Kısıtlama türü geçerli olmalıdır");
        RuleFor(x => x.RestrictionLevel)
            .InclusiveBetween((byte)0, (byte)2).WithMessage("Kısıtlama seviyesi geçerli olmalıdır");
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Başlangıç tarihi bugünden önce olmalıdır");
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır")
            .When(x => x.EndDate.HasValue);
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Neden boş olamaz")
            .MaximumLength(500).WithMessage("Neden maksimum 500 karakter olabilir");
        RuleFor(x => x.Severity)
            .InclusiveBetween(1, 10).WithMessage("Şiddeti 1 ile 10 arasında olmalıdır");
    }
}