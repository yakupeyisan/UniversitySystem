using FluentValidation;
using Identity.Application.Commands;

namespace Identity.Application.Validators;

public class RevokePermissionValidator : AbstractValidator<RevokePermissionCommand>
{
    public RevokePermissionValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Request).NotNull().WithMessage("Revoke permission request cannot be null");

        RuleFor(x => x.Request.PermissionId)
            .NotEmpty().WithMessage("Permission ID is required");
    }
}