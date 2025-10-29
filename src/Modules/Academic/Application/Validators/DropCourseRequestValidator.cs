using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class DropCourseRequestValidator : AbstractValidator<DropCourseRequest>
{
    public DropCourseRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ders b�rakma nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("Ders b�rakma nedeni maksimum 500 karakter olabilir");
    }
}