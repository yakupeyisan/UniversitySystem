using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class RecordGradeRequestValidator : AbstractValidator<RecordGradeRequest>
{
    public RecordGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Kayýt ID'si boþ olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester boþ olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara sýnav puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara sýnav puaný 100'den büyük olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puaný 100'den büyük olamaz");

        RuleFor(x => x.ECTS)
            .GreaterThanOrEqualTo(0).WithMessage("ECTS 0'dan küçük olamaz")
            .LessThanOrEqualTo(20).WithMessage("ECTS 20'den büyük olamaz");
    }
}