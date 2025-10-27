using FluentValidation;
using Identity.Application.Commands;

namespace Identity.Application.Validators;

public class RevokeRoleValidator : AbstractValidator<RevokeRoleCommand>
{
    public RevokeRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Request).NotNull().WithMessage("Revoke role request cannot be null");

        RuleFor(x => x.Request.RoleId)
            .NotEmpty().WithMessage("Role ID is required");
    }
}