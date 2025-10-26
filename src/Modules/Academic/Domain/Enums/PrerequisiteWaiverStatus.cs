namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing prerequisite waiver request status
/// </summary>
public enum PrerequisiteWaiverStatus
{
    /// <summary>
    /// Waiver request has been submitted
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Waiver request is under review
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Waiver has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Waiver has been denied
    /// </summary>
    Denied = 4,

    /// <summary>
    /// Waiver request has been withdrawn
    /// </summary>
    Withdrawn = 5,
    Pending=6
}