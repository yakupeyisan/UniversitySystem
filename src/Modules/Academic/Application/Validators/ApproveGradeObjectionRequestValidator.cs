using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class ApproveGradeObjectionRequestValidator : AbstractValidator<ApproveGradeObjectionRequest>
{
    public ApproveGradeObjectionRequestValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("Ýtiraz ID'si boþ olamaz");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty().WithMessage("Ýnceleyici ID'si boþ olamaz");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("Ýnceleme notlarý maksimum 1000 karakter olabilir");

        RuleFor(x => x.NewScore)
            .GreaterThanOrEqualTo(0).WithMessage("Yeni puan 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Yeni puan 100'den büyük olamaz")
            .When(x => x.NewScore.HasValue);
    }
}