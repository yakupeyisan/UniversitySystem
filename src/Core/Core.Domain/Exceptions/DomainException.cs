namespace Core.Domain.Exceptions;
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public abstract string ErrorCode { get; }
    public abstract int StatusCode { get; }
    public override string ToString()
    {
        return $"{GetType().Name} [{ErrorCode}] - {Message}";
    }
}