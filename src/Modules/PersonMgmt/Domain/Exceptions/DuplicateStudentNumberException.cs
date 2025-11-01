using Core.Domain.Exceptions;
namespace PersonMgmt.Domain.Exceptions;
public class DuplicateStudentNumberException : DomainException
{
    public DuplicateStudentNumberException(string studentNumber)
        : base($"Student with number {studentNumber} already exists.")
    {
        StudentNumber = studentNumber;
    }
    public string StudentNumber { get; }
    public override string ErrorCode => "errors.person.duplicate.student.number";
    public override int StatusCode => 409;
}