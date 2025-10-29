namespace Core.Domain.Filtering;

public class FilterParsingException : Exception
{
    public FilterParsingException(string message) : base(message)
    {
    }

    public FilterParsingException(string message, string filterString)
        : base(message)
    {
        FilterString = filterString;
    }

    public FilterParsingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public string? FilterString { get; }
}