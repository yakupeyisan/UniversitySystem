using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class AddPrerequisiteRequestValidator : AbstractValidator<AddPrerequisiteRequest>
{
    public AddPrerequisiteRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koþul kurs ID'si boþ olamaz");

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.CourseId == request.PrerequisiteCourseId)
                    context.AddFailure("Bir kurs kendisinin ön koþulu olamaz");
            });
    }
}