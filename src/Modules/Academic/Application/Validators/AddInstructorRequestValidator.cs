using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class AddInstructorRequestValidator : AbstractValidator<AddInstructorRequest>
{
    public AddInstructorRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Öðretmen ID'si boþ olamaz");
    }
}