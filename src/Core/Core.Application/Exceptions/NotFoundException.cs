namespace Core.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string resourceType, Guid resourceId)
        : base($"{resourceType} with ID '{resourceId}' was not found.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public override string ErrorCode => "RESOURCE_NOT_FOUND";
    public override int StatusCode => 404;
    public string? ResourceType { get; }
    public Guid? ResourceId { get; }
    public override object? Details => new { ResourceType, ResourceId };
}