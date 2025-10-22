namespace Core.Application.Exceptions;

/// <summary>
/// ConflictException - Business rule violation exception
/// 
/// Kullanım:
/// - Business logic rules'ı ihlal etme durumlarında throw et
/// - Domain layer'daki domain exceptions'ları wrap et
/// 
/// Örnekler:
/// throw new ConflictException("Person with this national ID already exists");
/// throw new ConflictException("Student is already enrolled in this course");
/// throw new ConflictException("Cannot update deleted person");
/// 
/// StatusCode: 409 Conflict (HTTP semantiğine uygun)
/// </summary>
public class ConflictException : ApplicationException
{
    public override string ErrorCode => "BUSINESS_RULE_VIOLATION";
    public override int StatusCode => 409; // Conflict

    /// <summary>
    /// ConflictException
    /// </summary>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// ConflictException with inner exception
    /// </summary>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Domain exception'ı wrap et
    /// </summary>
    public static ConflictException FromDomainException(Exception domainException)
    {
        return new ConflictException(
            domainException.Message,
            domainException);
    }
}