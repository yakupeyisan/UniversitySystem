using FluentValidation;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Validators;

public class HireStaffRequestValidator : AbstractValidator<HireStaffRequest>
{
    public HireStaffRequestValidator()
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("Personel numarası boş olamaz")
            .MaximumLength(20).WithMessage("Personel numarası maksimum 20 karakter olabilir");
        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Pozisyon boş olamaz")
            .MaximumLength(100).WithMessage("Pozisyon maksimum 100 karakter olabilir");
        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("İşe alınma tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("İşe alınma tarihi bugünden önce olmalıdır");
    }
}