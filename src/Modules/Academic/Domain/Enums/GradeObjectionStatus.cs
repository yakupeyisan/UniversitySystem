namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing grade objection/appeal status
/// </summary>
public enum GradeObjectionStatus
{
    /// <summary>
    /// Grade objection has been submitted
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Grade objection is under review
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Grade objection has been approved
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Grade objection has been rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Grade objection is pending further review/appeal
    /// </summary>
    Pending = 5,

    /// <summary>
    /// Grade objection has been withdrawn
    /// </summary>
    Withdrawn = 6
}