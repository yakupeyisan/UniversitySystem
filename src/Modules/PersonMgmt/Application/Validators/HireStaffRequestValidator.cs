using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

/// <summary>
/// HireStaffRequest validator
/// </summary>
public class HireStaffRequestValidator : AbstractValidator<HireStaffRequest>
{
    /// <summary>
    /// Constructor
    /// </summary>
    public HireStaffRequestValidator()
    {
        // Personel numarası
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Personel numarası boş olamaz")
            .MaximumLength(20).WithMessage("Personel numarası maksimum 20 karakter olabilir");

        // Pozisyon
        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Pozisyon boş olamaz")
            .MaximumLength(100).WithMessage("Pozisyon maksimum 100 karakter olabilir");

        // İşe alınma tarihi
        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("İşe alınma tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("İşe alınma tarihi bugünden önce olmalıdır");


        // Maaş
        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Maaş sıfırdan büyük veya eşit olmalıdır")
            .When(x => x.Salary.HasValue);
    }
}