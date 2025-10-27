using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class CancelCourseRequestValidator : AbstractValidator<CancelCourseRequest>
{
    public CancelCourseRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�ptal nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("�ptal nedeni maksimum 500 karakter olabilir");
    }
}