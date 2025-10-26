namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing student course registration status
/// </summary>
public enum RegistrationStatus
{
    /// <summary>
    /// Student is registered
    /// </summary>
    Registered = 1,

    /// <summary>
    /// Student dropped the course
    /// </summary>
    Dropped = 2,

    /// <summary>
    /// Student completed the course
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Student failed the course
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Student is on waiting list
    /// </summary>
    WaitingList = 5,

    /// <summary>
    /// Registration is pending approval
    /// </summary>
    Pending = 6
}