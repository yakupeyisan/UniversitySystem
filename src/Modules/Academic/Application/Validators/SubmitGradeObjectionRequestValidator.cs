using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class SubmitGradeObjectionRequestValidator : AbstractValidator<SubmitGradeObjectionRequest>
{
    public SubmitGradeObjectionRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si boþ olamaz");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýtirazi neden boþ olamaz")
            .MaximumLength(500).WithMessage("Ýtirazi neden maksimum 500 karakter olabilir");
    }
}