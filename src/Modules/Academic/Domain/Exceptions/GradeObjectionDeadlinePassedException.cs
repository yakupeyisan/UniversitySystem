using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

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