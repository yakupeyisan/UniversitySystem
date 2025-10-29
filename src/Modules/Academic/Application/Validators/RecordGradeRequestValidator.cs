using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class RecordGradeRequestValidator : AbstractValidator<RecordGradeRequest>
{
    public RecordGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Kay�t ID'si bo� olamaz");
        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester bo� olamaz");
        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara s�nav puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara s�nav puan� 100'den b�y�k olamaz");
        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puan� 100'den b�y�k olamaz");
        RuleFor(x => x.ECTS)
            .GreaterThanOrEqualTo(0).WithMessage("ECTS 0'dan k���k olamaz")
            .LessThanOrEqualTo(20).WithMessage("ECTS 20'den b�y�k olamaz");
    }
}