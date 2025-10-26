namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing course status
/// </summary>
public enum CourseStatus
{
    /// <summary>
    /// Course is active and accepting registrations
    /// </summary>
    Active = 1,

    /// <summary>
    /// Course is inactive
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Course has been cancelled
    /// </summary>
    Cancelled = 3
}