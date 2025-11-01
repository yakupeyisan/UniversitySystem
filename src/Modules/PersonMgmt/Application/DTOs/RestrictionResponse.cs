namespace PersonMgmt.Application.DTOs;
public class RestrictionResponse
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string RestrictionType { get; set; } = string.Empty;
    public string RestrictionLevel { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int Severity { get; set; }
    public bool IsActive { get; set; }
    public Guid AppliedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}