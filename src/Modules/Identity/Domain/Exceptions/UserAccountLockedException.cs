using Core.Domain.Exceptions;

namespace Identity.Domain.Exceptions;

public class UserAccountLockedException : DomainException
{
    public override string ErrorCode => "IDT009";
    public override int StatusCode => 423;

    public UserAccountLockedException(Guid userId)
        : base($"User account {userId} is locked") { }
}