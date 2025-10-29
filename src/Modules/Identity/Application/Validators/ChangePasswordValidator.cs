using FluentValidation;
using Identity.Application.Commands;

namespace Identity.Application.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Request).NotNull().WithMessage("Change password request cannot be null");

        RuleFor(x => x.Request.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*)")
            .NotEqual(x => x.Request.CurrentPassword)
            .WithMessage("New password must be different from current password");
    }
}