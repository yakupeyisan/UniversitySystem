using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// 🆕 NEW: Aynı Student Number'a sahip kişi zaten varsa throw edilen exception
/// </summary>
public class DuplicateStudentNumberException : DomainException
{
    public string StudentNumber { get; }

    public DuplicateStudentNumberException(string studentNumber)
        : base($"Student with number {studentNumber} already exists.")
    {
        StudentNumber = studentNumber;
    }

    public override string ErrorCode => "errors.person.duplicate.student.number";
    public override int StatusCode => 409;
}