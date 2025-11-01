using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class PostponeExamRequestValidator : AbstractValidator<PostponeExamRequest>
{
    public PostponeExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");
        RuleFor(x => x.NewExamDate)
            .NotEmpty().WithMessage("Yeni s�nav tarihi bo� olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Yeni s�nav tarihi format hatas� (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Yeni s�nav tarihi ge�mi� tarih olamaz");
                }
                else
                {
                    context.AddFailure("Yeni s�nav tarihi ge�ersiz");
                }
            });
        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("Yeni ba�lang�� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni ba�lang�� saati format hatas� (HH:mm)");
        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("Yeni biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni biti� saati format hatas� (HH:mm)");
    }
}