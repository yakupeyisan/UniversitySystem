using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
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
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}