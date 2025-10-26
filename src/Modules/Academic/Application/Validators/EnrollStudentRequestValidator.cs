using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequest>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester bo� olamaz");
    }
}