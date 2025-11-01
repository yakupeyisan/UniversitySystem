namespace Academic.Application.DTOs;
public class PrerequisiteWaiverResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid PrerequisiteId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? ApprovalNotes { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}