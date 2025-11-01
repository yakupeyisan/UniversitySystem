using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class AddInstructorRequestValidator : AbstractValidator<AddInstructorRequest>
{
    public AddInstructorRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("��retmen ID'si bo� olamaz");
    }
}