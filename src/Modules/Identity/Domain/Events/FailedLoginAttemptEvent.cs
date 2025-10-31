using Core.Domain.Events;

namespace Identity.Domain.Events;

public class FailedLoginAttemptEvent : DomainEvent
{
    public FailedLoginAttemptEvent(string email, string ipAddress, string reason)
    {
        Email = email;
        IpAddress = ipAddress;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }

    public string Email { get; }
    public string IpAddress { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Parola s�f�rland� event'i
/// </summary>
public class UserPasswordResetEvent : DomainEvent
{
    public UserPasswordResetEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Silinen kullan�c� geri al�nd� event'i
/// </summary>
public class UserRestoredEvent : DomainEvent
{
    public UserRestoredEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Role atand� event'i
/// </summary>
public class RoleAssignedToUserEvent : DomainEvent
{
    public RoleAssignedToUserEvent(Guid userId, Guid roleId, string roleName)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public Guid RoleId { get; }
    public string RoleName { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// Role kald�r�ld� event'i
/// </summary>
public class RoleRemovedFromUserEvent : DomainEvent
{
    public RoleRemovedFromUserEvent(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public Guid RoleId { get; }
    public DateTime OccurredOn { get; }
}

/// <summary>
/// �ok fazla ba�ar�s�z giri� denemesi event'i
/// </summary>
public class ExcessiveFailedLoginAttemptsEvent : DomainEvent
{
    public ExcessiveFailedLoginAttemptsEvent(Guid userId, string email, int attemptCount)
    {
        UserId = userId;
        Email = email;
        AttemptCount = attemptCount;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public int AttemptCount { get; }
    public DateTime OccurredOn { get; }
}