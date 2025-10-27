using Core.Domain;
using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserCreatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserUpdatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserUpdatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserDeletedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserDeletedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserPasswordChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserPasswordChangedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserStatusChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public int Status { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserStatusChangedEvent(Guid userId, string email, int status)
    {
        UserId = userId;
        Email = email;
        Status = status;
    }
}

public class UserRoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserRoleAssignedEvent(Guid userId, string email, string roleName)
    {
        UserId = userId;
        Email = email;
        RoleName = roleName;
    }
}

public class UserRoleRevokedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserRoleRevokedEvent(Guid userId, string email, string roleName)
    {
        UserId = userId;
        Email = email;
        RoleName = roleName;
    }
}

public class UserEmailVerifiedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserEmailVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserLockedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime LockedUntil { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLockedEvent(Guid userId, string email, DateTime lockedUntil)
    {
        UserId = userId;
        Email = email;
        LockedUntil = lockedUntil;
    }
}

public class UserUnlockedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserUnlockedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserLoginFailedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoginFailedEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
    }
}

public class UserSuccessfulLoginEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserSuccessfulLoginEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}
