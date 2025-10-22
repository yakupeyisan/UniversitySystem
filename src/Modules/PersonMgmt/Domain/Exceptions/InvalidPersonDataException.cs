using Core.Domain.Exceptions;

namespace PersonMgmt.Domain.Exceptions;

/// <summary>
/// Geçersiz Person verisi gönderildiğinde throw edilen exception
/// </summary>
public class InvalidPersonDataException : DomainException
{
    public InvalidPersonDataException(string message)
        : base(message)
    {
    }

    public override string ErrorCode => "errors.person.invalid.data";
    public override int StatusCode => 400;
}