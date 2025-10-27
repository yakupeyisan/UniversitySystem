using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class StudentAlreadyRegisteredException : DomainException
{
    public override string ErrorCode => "ACD003";
    public override int StatusCode => 409;
    public StudentAlreadyRegisteredException(Guid studentId, Guid courseId)
        : base($"Student {studentId} is already registered for course {courseId}.") { }
}