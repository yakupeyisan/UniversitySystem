using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when course capacity is exceeded
/// </summary>
public class CourseCapacityExceededException : DomainException
{
    public Guid CourseId { get; }

    public CourseCapacityExceededException(Guid courseId)
        : base($"Course capacity has been exceeded for course ID {courseId}.")
    {
        CourseId = courseId;
    }

    public override string ErrorCode => "errors.course.capacity.exceeded";
    public override int StatusCode => 409;
}

/// <summary>
/// Exception thrown when a grade is not found
/// </summary>
public class GradeNotFoundException : DomainException
{
    public Guid GradeId { get; }

    public GradeNotFoundException(Guid gradeId)
        : base($"Grade with ID {gradeId} was not found.")
    {
        GradeId = gradeId;
    }

    public override string ErrorCode => "errors.grade.not.found";
    public override int StatusCode => 404;
}

/// <summary>
/// Exception thrown when a course registration is not found
/// </summary>
public class CourseRegistrationNotFoundException : DomainException
{
    public Guid RegistrationId { get; }

    public CourseRegistrationNotFoundException(Guid registrationId)
        : base($"Course registration with ID {registrationId} was not found.")
    {
        RegistrationId = registrationId;
    }

    public override string ErrorCode => "errors.course.registration.not.found";
    public override int StatusCode => 404;
}

/// <summary>
/// Exception thrown when an exam is not found
/// </summary>
public class ExamNotFoundException : DomainException
{
    public Guid ExamId { get; }

    public ExamNotFoundException(Guid examId)
        : base($"Exam with ID {examId} was not found.")
    {
        ExamId = examId;
    }

    public override string ErrorCode => "errors.exam.not.found";
    public override int StatusCode => 404;
}

/// <summary>
/// Exception thrown when a grade objection is not found
/// </summary>
public class GradeObjectionNotFoundException : DomainException
{
    public Guid ObjectionId { get; }

    public GradeObjectionNotFoundException(Guid objectionId)
        : base($"Grade objection with ID {objectionId} was not found.")
    {
        ObjectionId = objectionId;
    }

    public override string ErrorCode => "errors.grade.objection.not.found";
    public override int StatusCode => 404;
}

/// <summary>
/// Exception thrown when grade objection deadline has passed
/// </summary>
public class GradeObjectionDeadlinePassedException : DomainException
{
    public DateTime DeadlineDate { get; }

    public GradeObjectionDeadlinePassedException(DateTime deadlineDate)
        : base($"Grade objection deadline has passed. Deadline was {deadlineDate:yyyy-MM-dd}.")
    {
        DeadlineDate = deadlineDate;
    }

    public override string ErrorCode => "errors.grade.objection.deadline.passed";
    public override int StatusCode => 409;
}
