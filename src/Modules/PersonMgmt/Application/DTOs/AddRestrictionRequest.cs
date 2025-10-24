namespace PersonMgmt.Application.DTOs;
public class AddRestrictionRequest
{
    public byte RestrictionType { get; set; }
    public byte RestrictionLevel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Reason { get; set; }
    public int Severity { get; set; }
}