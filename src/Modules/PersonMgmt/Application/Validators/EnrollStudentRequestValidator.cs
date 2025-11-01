using FluentValidation;
using PersonMgmt.Application.DTOs;
namespace PersonMgmt.Application.Validators;
public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequest>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Öğrenci numarası boş olamaz")
            .MaximumLength(20).WithMessage("Öğrenci numarası maksimum 20 karakter olabilir");
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID boş olamaz");
        RuleFor(x => x.EnrollmentDate)
            .NotEmpty().WithMessage("Kayıt tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Kayıt tarihi bugünden önce olmalıdır");
        RuleFor(x => x.EducationLevel)
            .InclusiveBetween((byte)0, (byte)2)
            .WithMessage("Eğitim düzeyi 0 (Lisans), 1 (Yüksek Lisans) veya 2 (Doktora) olmalıdır");
    }
}