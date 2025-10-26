using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class DropCourseRequestValidator : AbstractValidator<DropCourseRequest>
{
    public DropCourseRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ders býrakma nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ders býrakma nedeni maksimum 500 karakter olabilir");
    }
}