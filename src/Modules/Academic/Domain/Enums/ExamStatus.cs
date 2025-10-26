namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing exam status
/// </summary>
public enum ExamStatus
{
    /// <summary>
    /// Exam has been scheduled
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Exam is in progress
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Exam has been completed
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Exam has been cancelled
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Exam results are being graded
    /// </summary>
    Grading = 5
}