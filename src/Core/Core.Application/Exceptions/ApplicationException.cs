namespace Core.Application.Exceptions;

/// <summary>
/// ApplicationException - Application layer'ın base exception'ı
/// 
/// Kullanım:
/// - Application business logic hatalarında throw et
/// - Domain layer'dan "throw"la gelen Domain Exceptions'ları wrap et
/// 
/// Örnekler:
/// - ValidationException (Validator fail)
/// - NotFoundException (Entity not found)
/// - ConflictException (Business rule violation)
/// 
/// Not: Domain.DomainException'dan farklı
/// - DomainException: Domain layer'da throw edilir
/// - ApplicationException: Application layer'da throw edilir
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Exception'ın error code'u (standardized)
    /// </summary>
    public abstract string ErrorCode { get; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public abstract int StatusCode { get; }

    /// <summary>
    /// Ek detaylar (error messages, validation errors, vb.)
    /// </summary>
    public virtual object? Details { get; }

    protected ApplicationException(string message) : base(message)
    {
    }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{GetType().Name} [{ErrorCode}] ({StatusCode}) - {Message}";
    }
}