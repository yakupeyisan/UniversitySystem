namespace PersonMgmt.Application.DTOs;
public class HealthRecordResponse
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? Medications { get; set; }
    public string? EmergencyHealthInfo { get; set; }
    public DateTime? LastCheckupDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}