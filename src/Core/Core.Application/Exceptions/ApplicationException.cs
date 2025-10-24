namespace Core.Application.Exceptions;
public abstract class ApplicationException : Exception
{
    public abstract string ErrorCode { get; }
    public abstract int StatusCode { get; }
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