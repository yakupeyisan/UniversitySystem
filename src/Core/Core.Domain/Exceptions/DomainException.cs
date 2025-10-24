namespace Core.Domain.Exceptions;
public abstract class DomainException : Exception
{
    public abstract string ErrorCode { get; }
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