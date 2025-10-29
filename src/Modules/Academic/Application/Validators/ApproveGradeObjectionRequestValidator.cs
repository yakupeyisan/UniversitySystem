using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class ApproveGradeObjectionRequestValidator : AbstractValidator<ApproveGradeObjectionRequest>
{
    public ApproveGradeObjectionRequestValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("�tiraz ID'si bo� olamaz");
        RuleFor(x => x.ReviewedBy)
            .NotEmpty().WithMessage("�nceleyici ID'si bo� olamaz");
        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("�nceleme notlar� maksimum 1000 karakter olabilir");
        RuleFor(x => x.NewScore)
            .GreaterThanOrEqualTo(0).WithMessage("Yeni puan 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Yeni puan 100'den b�y�k olamaz")
            .When(x => x.NewScore.HasValue);
    }
}