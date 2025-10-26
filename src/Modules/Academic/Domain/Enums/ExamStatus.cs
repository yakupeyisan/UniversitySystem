namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing exam status
/// </summary>
public enum ExamStatus
{
    /// <summary>
    /// Exam is scheduled
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Exam is in progress
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Exam is completed
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Exam is cancelled
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Exam is postponed
    /// </summary>
    Postponed = 5
}