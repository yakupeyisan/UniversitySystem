using FluentValidation;
using Academic.Application.DTOs;
namespace Academic.Application.Validators;
public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Kurs kodu bo� olamaz")
            .Length(4, 20).WithMessage("Kurs kodu 4-20 karakter aras�nda olmal�d�r")
            .Matches(@"^[A-Z]{2,4}\d{2,4}$").WithMessage("Kurs kodu format hatas� (�rn: CS101)");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs ad� bo� olamaz")
            .Length(3, 200).WithMessage("Kurs ad� 3-200 karakter aras�nda olmal�d�r");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A��klama maksimum 1000 karakter olabilir");
        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");
        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");
        RuleFor(x => x.Level)
            .InclusiveBetween(1, 4).WithMessage("Seviye 1-4 aras�nda olmal�d�r");
        RuleFor(x => x.Type)
            .InclusiveBetween(1, 5).WithMessage("T�r 1-5 aras�nda olmal�d�r");
        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 3).WithMessage("Semester 1-3 aras�nda olmal�d�r");
        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Y�l 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(4).WithMessage("Y�l maksimum 4 olabilir");
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("B�l�m ID'si bo� olamaz");
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}