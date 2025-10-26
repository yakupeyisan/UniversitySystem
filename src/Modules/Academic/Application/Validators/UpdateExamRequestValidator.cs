using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
{
    public UpdateExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("Sýnav tarihi boþ olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Sýnav tarihi format hatasý (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Sýnav tarihi geçmiþ tarih olamaz");
                }
                else
                    context.AddFailure("Sýnav tarihi geçersiz");
            });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Baþlangýç saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Baþlangýç saati format hatasý (HH:mm)");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Bitiþ saati format hatasý (HH:mm)");
    }
}