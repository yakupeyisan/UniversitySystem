using FluentValidation;
using Identity.Application.Commands;
namespace Identity.Application.Validators;
public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
        RuleFor(x => x.Request).NotNull().WithMessage("Assign role request cannot be null");
        RuleFor(x => x.Request.RoleId)
            .NotEmpty().WithMessage("Role ID is required");
    }
}