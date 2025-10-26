namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing grade objection status
/// </summary>
public enum GradeObjectionStatus
{
    /// <summary>
    /// Objection is submitted
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Objection is under review
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Objection is approved and grade updated
    /// </summary>
    Approved = 3,

    /// <summary>
    /// Objection is rejected
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// Objection is pending (not yet processed)
    /// </summary>
    Pending = 5,

    /// <summary>
    /// Objection is escalated to higher level
    /// </summary>
    Escalated = 6
}
