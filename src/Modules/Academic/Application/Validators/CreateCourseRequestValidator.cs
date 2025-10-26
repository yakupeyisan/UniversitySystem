using FluentValidation;
using Academic.Application.DTOs;

namespace Academic.Application.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Kurs kodu boþ olamaz")
            .Length(4, 20).WithMessage("Kurs kodu 4-20 karakter arasýnda olmalýdýr")
            .Matches(@"^[A-Z]{2,4}\d{2,4}$").WithMessage("Kurs kodu format hatasý (örn: CS101)");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs adý boþ olamaz")
            .Length(3, 200).WithMessage("Kurs adý 3-200 karakter arasýnda olmalýdýr");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açýklama maksimum 1000 karakter olabilir");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 4).WithMessage("Seviye 1-4 arasýnda olmalýdýr");

        RuleFor(x => x.Type)
            .InclusiveBetween(1, 5).WithMessage("Tür 1-5 arasýnda olmalýdýr");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 3).WithMessage("Semester 1-3 arasýnda olmalýdýr");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Yýl 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(4).WithMessage("Yýl maksimum 4 olabilir");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm ID'si boþ olamaz");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}