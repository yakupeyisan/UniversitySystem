using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class CancelExamRequestValidator : AbstractValidator<CancelExamRequest>
{
    public CancelExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�ptal nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("�ptal nedeni maksimum 500 karakter olabilir");
    }
}