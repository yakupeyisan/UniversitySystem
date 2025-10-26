using Academic.Application.DTOs;
using FluentValidation;

namespace Academic.Application.Validators;

public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequest>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si boþ olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara sýnav puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara sýnav puaný 100'den büyük olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puaný 100'den büyük olamaz");
    }
}