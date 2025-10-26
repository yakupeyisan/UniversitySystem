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

public class UpdateCourseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public int MaxCapacity { get; set; }
}

public class AddInstructorRequest
{
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
}

public class RemoveInstructorRequest
{
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
}

public class AddPrerequisiteRequest
{
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
}

public class CancelCourseRequest
{
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// ============================================================
// COURSE RESPONSE DTOs
// ============================================================

public class CourseResponse
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ECTS { get; set; }
    public int Credits { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public int Year { get; set; }
    public Guid DepartmentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public int CurrentEnrollment { get; set; }
    public float OccupancyPercentage { get; set; }
    public int InstructorCount { get; set; }
    public int PrerequisiteCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CourseListResponse
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public int ECTS { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CurrentEnrollment { get; set; }
    public int MaxCapacity { get; set; }
}


// ============================================================
// GRADE REQUEST DTOs
// ============================================================

public class RecordGradeRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid RegistrationId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
    public int ECTS { get; set; }
}

public class UpdateGradeRequest
{
    public Guid GradeId { get; set; }
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
}

public class SubmitGradeObjectionRequest
{
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ApproveGradeObjectionRequest
{
    public Guid ObjectionId { get; set; }
    public Guid ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
    public float? NewScore { get; set; }
    public int? NewLetterGrade { get; set; }
}

public class RejectGradeObjectionRequest
{
    public Guid ObjectionId { get; set; }
    public Guid ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
}

// ============================================================
// GRADE RESPONSE DTOs
// ============================================================

public class GradeResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
    public float NumericScore { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public float GradePoint { get; set; }
    public int ECTS { get; set; }
    public bool IsObjected { get; set; }
    public DateTime ObjectionDeadline { get; set; }
    public DateTime RecordedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class StudentGradesResponse
{
    public Guid StudentId { get; set; }
    public List<GradeResponse> Grades { get; set; } = new();
    public float CumulativeGPA { get; set; }
    public float TotalECTS { get; set; }
}

public class GradeObjectionResponse
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime ObjectionDate { get; set; }
    public DateTime ObjectionDeadline { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AppealLevel { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewNotes { get; set; }
    public float? NewScore { get; set; }
    public string? NewLetterGrade { get; set; }
}


// ============================================================
// ENROLLMENT REQUEST DTOs
// ============================================================

public class EnrollStudentRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public bool IsRetake { get; set; }
    public Guid? PreviousGradeId { get; set; }
}

public class DropCourseRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class JoinWaitingListRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
}

// ============================================================
// ENROLLMENT RESPONSE DTOs
// ============================================================

public class CourseRegistrationResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DropDate { get; set; }
    public string? DropReason { get; set; }
    public bool IsRetake { get; set; }
    public Guid? GradeId { get; set; }
}

public class StudentCoursesResponse
{
    public Guid StudentId { get; set; }
    public List<CourseRegistrationResponse> Courses { get; set; } = new();
    public int TotalEnrolledCourses { get; set; }
    public int TotalECTS { get; set; }
}

public class WaitingListResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int QueuePosition { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public DateTime? AdmittedDate { get; set; }
}

// ============================================================
// EXAM REQUEST DTOs
// ============================================================

public class ScheduleExamRequest
{
    public Guid CourseId { get; set; }
    public int ExamType { get; set; }
    public string ExamDate { get; set; } = string.Empty; // Date format: yyyy-MM-dd
    public string StartTime { get; set; } = string.Empty; // Time format: HH:mm
    public string EndTime { get; set; } = string.Empty;   // Time format: HH:mm
    public int MaxCapacity { get; set; }
    public Guid? ExamRoomId { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
}

public class UpdateExamRequest
{
    public Guid ExamId { get; set; }
    public string ExamDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public Guid? ExamRoomId { get; set; }
}

public class PostponeExamRequest
{
    public Guid ExamId { get; set; }
    public string NewExamDate { get; set; } = string.Empty;
    public string NewStartTime { get; set; } = string.Empty;
    public string NewEndTime { get; set; } = string.Empty;
}

public class CancelExamRequest
{
    public Guid ExamId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// ============================================================
// EXAM RESPONSE DTOs
// ============================================================

public class ExamResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public string ExamDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public Guid? ExamRoomId { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentRegisteredCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
}

public class CourseExamsResponse
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<ExamResponse> Exams { get; set; } = new();
}
