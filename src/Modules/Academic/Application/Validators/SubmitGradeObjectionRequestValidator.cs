using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class SubmitGradeObjectionRequestValidator : AbstractValidator<SubmitGradeObjectionRequest>
{
    public SubmitGradeObjectionRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si bo� olamaz");
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�tirazi neden bo� olamaz")
            .MaximumLength(500).WithMessage("�tirazi neden maksimum 500 karakter olabilir");
    }
}