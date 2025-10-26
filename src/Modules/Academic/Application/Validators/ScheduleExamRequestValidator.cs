using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class ScheduleExamRequestValidator : AbstractValidator<ScheduleExamRequest>
{
    public ScheduleExamRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.ExamType)
            .InclusiveBetween(1, 5).WithMessage("Sýnav türü 1-5 arasýnda olmalýdýr");

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
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Baþlangýç saati format hatasý (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Baþlangýç saati geçersiz");
            });

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Bitiþ saati format hatasý (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Bitiþ saati geçersiz");
            });

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (TimeOnly.TryParse(request.StartTime, out var start) &&
                    TimeOnly.TryParse(request.EndTime, out var end))
                {
                    if (end <= start)
                        context.AddFailure("Bitiþ saati baþlangýç saatinden sonra olmalýdýr");
                }
            });

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");

        RuleFor(x => x.IsOnline)
            .Custom((isOnline, context) =>
            {
                var request = context.InstanceToValidate;
                if (isOnline && string.IsNullOrWhiteSpace(request.OnlineLink))
                    context.AddFailure("Online sýnav için link saðlanmalýdýr");
            });

        RuleFor(x => x.OnlineLink)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Online link geçerli bir URL olmalýdýr")
            .When(x => !string.IsNullOrWhiteSpace(x.OnlineLink));
    }
}