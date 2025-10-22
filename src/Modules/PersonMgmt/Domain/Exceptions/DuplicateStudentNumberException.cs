using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// Student number'ı zaten kullanımda olduğunda throw edilen exception
/// </summary>
public class DuplicateStudentNumberException : DomainException
{
    public string StudentNumber { get; }

    public DuplicateStudentNumberException(string studentNumber)
        : base($"Student number {studentNumber} is already in use.")
    {
        StudentNumber = studentNumber;
    }

    public override string ErrorCode => "errors.student.number.duplicate";
    public override int StatusCode => 409;
}