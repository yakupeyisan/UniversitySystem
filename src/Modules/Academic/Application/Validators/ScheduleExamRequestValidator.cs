using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class ScheduleExamRequestValidator : AbstractValidator<ScheduleExamRequest>
{
    public ScheduleExamRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.ExamType)
            .InclusiveBetween(1, 5).WithMessage("S�nav t�r� 1-5 aras�nda olmal�d�r");
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
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Ba�lang�� saati format hatas� (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Ba�lang�� saati ge�ersiz");
            });
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Biti� saati format hatas� (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Biti� saati ge�ersiz");
            });
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (TimeOnly.TryParse(request.StartTime, out var start) &&
                    TimeOnly.TryParse(request.EndTime, out var end))
                    if (end <= start)
                        context.AddFailure("Biti� saati ba�lang�� saatinden sonra olmal�d�r");
            });
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
        RuleFor(x => x.IsOnline)
            .Custom((isOnline, context) =>
            {
                var request = context.InstanceToValidate;
                if (isOnline && string.IsNullOrWhiteSpace(request.OnlineLink))
                    context.AddFailure("Online s�nav i�in link sa�lanmal�d�r");
            });
        RuleFor(x => x.OnlineLink)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Online link ge�erli bir URL olmal�d�r")
            .When(x => !string.IsNullOrWhiteSpace(x.OnlineLink));
    }
}