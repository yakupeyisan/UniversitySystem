using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class CancelExamRequestValidator : AbstractValidator<CancelExamRequest>
{
    public CancelExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýptal nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ýptal nedeni maksimum 500 karakter olabilir");
    }
}