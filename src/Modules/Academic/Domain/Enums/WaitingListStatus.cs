namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing waiting list entry status
/// </summary>
public enum WaitingListStatus
{
    /// <summary>
    /// Student is waiting for a seat
    /// </summary>
    Waiting = 1,

    /// <summary>
    /// Student has been admitted from the waiting list
    /// </summary>
    Admitted = 2,

    /// <summary>
    /// Waiting list entry has been cancelled
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Waiter has been rejected
    /// </summary>
    Rejected = 4
}