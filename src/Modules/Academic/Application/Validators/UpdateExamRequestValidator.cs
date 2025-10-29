using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
{
    public UpdateExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");
        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("S�nav tarihi bo� olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("S�nav tarihi format hatas� (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("S�nav tarihi ge�mi� tarih olamaz");
                }
                else
                {
                    context.AddFailure("S�nav tarihi ge�ersiz");
                }
            });
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Ba�lang�� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Ba�lang�� saati format hatas� (HH:mm)");
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Biti� saati format hatas� (HH:mm)");
    }
}