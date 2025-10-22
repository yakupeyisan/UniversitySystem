namespace Core.Application.Exceptions;

/// <summary>
/// NotFoundException - Kaynak bulunamadı exception
/// 
/// Kullanım:
/// - Entity database'de bulunamadığında throw et
/// 
/// Örnekler:
/// throw new NotFoundException("Person", personId);
/// throw new NotFoundException("Student with email test@test.com not found");
/// 
/// Details property:
/// { "resourceType": "Person", "resourceId": "guid" }
/// </summary>
public class NotFoundException : ApplicationException
{
    public override string ErrorCode => "RESOURCE_NOT_FOUND";
    public override int StatusCode => 404;

    /// <summary>
    /// Bulunamayan kaynak tipi ve ID
    /// </summary>
    public string? ResourceType { get; }
    public Guid? ResourceId { get; }

    public override object? Details => new { ResourceType, ResourceId };

    /// <summary>
    /// ResourceType ve ResourceId ile exception
    /// </summary>
    public NotFoundException(string resourceType, Guid resourceId)
        : base($"{resourceType} with ID '{resourceId}' was not found.")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// Custom message ile exception
    /// </summary>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Inner exception ile exception
    /// </summary>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}