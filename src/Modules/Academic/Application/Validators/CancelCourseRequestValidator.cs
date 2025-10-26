using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class CancelCourseRequestValidator : AbstractValidator<CancelCourseRequest>
{
    public CancelCourseRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýptal nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ýptal nedeni maksimum 500 karakter olabilir");
    }
}