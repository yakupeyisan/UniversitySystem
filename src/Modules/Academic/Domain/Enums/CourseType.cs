namespace Academic.Domain.Enums;

/// <summary>
/// Enum representing course types/requirements
/// </summary>
public enum CourseType
{
    /// <summary>
    /// Required course - must be taken
    /// </summary>
    Compulsory = 1,

    /// <summary>
    /// Elective course - student can choose
    /// </summary>
    Elective = 2,

    /// <summary>
    /// Optional course - additional choice
    /// </summary>
    Optional = 3
}