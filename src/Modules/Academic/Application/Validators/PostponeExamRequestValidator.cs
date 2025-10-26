using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class PostponeExamRequestValidator : AbstractValidator<PostponeExamRequest>
{
    public PostponeExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.NewExamDate)
            .NotEmpty().WithMessage("Yeni sýnav tarihi boþ olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Yeni sýnav tarihi format hatasý (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Yeni sýnav tarihi geçmiþ tarih olamaz");
                }
                else
                    context.AddFailure("Yeni sýnav tarihi geçersiz");
            });

        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("Yeni baþlangýç saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni baþlangýç saati format hatasý (HH:mm)");

        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("Yeni bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni bitiþ saati format hatasý (HH:mm)");
    }
}