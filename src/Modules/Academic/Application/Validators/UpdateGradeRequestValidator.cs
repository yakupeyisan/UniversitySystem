using Academic.Application.DTOs;
using FluentValidation;
namespace Academic.Application.Validators;
public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequest>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si bo� olamaz");
        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara s�nav puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara s�nav puan� 100'den b�y�k olamaz");
        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puan� 100'den b�y�k olamaz");
    }
}