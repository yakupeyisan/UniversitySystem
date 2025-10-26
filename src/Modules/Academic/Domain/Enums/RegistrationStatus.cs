namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing student course registration status
/// </summary>
public enum RegistrationStatus
{
    /// <summary>
    /// Student is actively enrolled
    /// </summary>
    Active = 1,

    /// <summary>
    /// Student has dropped the course
    /// </summary>
    Dropped = 2,

    /// <summary>
    /// Student has withdrawn from the course
    /// </summary>
    Withdrawn = 3,

    /// <summary>
    /// Course has been completed
    /// </summary>
    Completed = 4
}