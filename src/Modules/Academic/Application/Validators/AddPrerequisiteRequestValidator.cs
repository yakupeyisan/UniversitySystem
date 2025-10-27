using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class AddPrerequisiteRequestValidator : AbstractValidator<AddPrerequisiteRequest>
{
    public AddPrerequisiteRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("�n ko�ul kurs ID'si bo� olamaz");
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.CourseId == request.PrerequisiteCourseId)
                    context.AddFailure("Bir kurs kendisinin �n ko�ulu olamaz");
            });
    }
}