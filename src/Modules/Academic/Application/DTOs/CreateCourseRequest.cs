namespace Academic.Application.DTOs;

// ============================================================
// COURSE REQUEST DTOs
// ============================================================

public class CreateCourseRequest
{
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public int Level { get; set; } // CourseLevel enum value
    public int Type { get; set; } // CourseType enum value
    public int Semester { get; set; } // CourseSemester enum value
    public int Year { get; set; }
    public Guid DepartmentId { get; set; }
    public int MaxCapacity { get; set; }
}

// ============================================================
// COURSE RESPONSE DTOs
// ============================================================

// ============================================================
// GRADE REQUEST DTOs
// ============================================================

// ============================================================
// GRADE RESPONSE DTOs
// ============================================================

// ============================================================
// ENROLLMENT REQUEST DTOs
// ============================================================

// ============================================================
// ENROLLMENT RESPONSE DTOs
// ============================================================

// ============================================================
// EXAM REQUEST DTOs
// ============================================================

// ============================================================
// EXAM RESPONSE DTOs
// ============================================================