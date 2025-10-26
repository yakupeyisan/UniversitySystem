namespace Academic.Application.DTOs;

public class RequestPrerequisiteWaiverRequest
{
    public Guid StudentId { get; set; }
    public Guid PrerequisiteId { get; set; }
    public string Reason { get; set; } = string.Empty;
}