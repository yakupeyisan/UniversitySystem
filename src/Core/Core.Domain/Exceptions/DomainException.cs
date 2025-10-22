namespace Core.Domain.Exceptions;

/// <summary>
/// DomainException - Domain-specific exceptions
/// 
/// Kullanım amacı:
/// - Business rule violations
/// - Invalid state transitions
/// - Domain constraints violations
/// 
/// Örnekler:
/// - InsufficientBalanceException
/// - PersonNotFoundException
/// - DuplicateNationalIdException
/// 
/// Not: Bu exception'lar domain logic'ten throw edilir
/// ve application layer'da handle edilir
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Exception'ın error code'u (standardized error handling için)
    /// </summary>
    public abstract string ErrorCode { get; }

    /// <summary>
    /// HTTP status code (API response'unda kullanılacak)
    /// </summary>
    public abstract int StatusCode { get; }

    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{GetType().Name} [{ErrorCode}] - {Message}";
    }
}