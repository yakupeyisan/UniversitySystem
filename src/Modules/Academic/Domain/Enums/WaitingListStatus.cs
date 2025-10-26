namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing waiting list status
/// </summary>
public enum WaitingListStatus
{
    /// <summary>
    /// Student is waiting
    /// </summary>
    Waiting = 1,

    /// <summary>
    /// Student is admitted from waiting list
    /// </summary>
    Admitted = 2,

    /// <summary>
    /// Student cancelled waiting
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Waiting list entry expired
    /// </summary>
    Expired = 4
}
