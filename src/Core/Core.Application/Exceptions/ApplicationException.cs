namespace Core.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message)
    {
    }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public abstract string ErrorCode { get; }
    public abstract int StatusCode { get; }
    public virtual object? Details { get; }

    public override string ToString()
    {
        return $"{GetType().Name} [{ErrorCode}] ({StatusCode}) - {Message}";
    }
}