using Core.Domain;
namespace PersonMgmt.Domain.Aggregates;
public class HealthRecord : AuditableEntity
{
    public Guid PersonId { get; private set; }
    public string? BloodType { get; private set; }
    public string? Allergies { get; private set; }
    public string? ChronicDiseases { get; private set; }
    public string? Medications { get; private set; }
    public string? EmergencyHealthInfo { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? LastCheckupDate { get; private set; }
    public bool IsDeleted { get; private set; }
    private HealthRecord()
    {
    }
    public static HealthRecord Create(
        Guid personId,
    string? bloodType = null,
    string? allergies = null,
    string? chronicDiseases = null,
    string? medications = null,
    string? emergencyHealthInfo = null,
    string? notes = null)
    {
        return new HealthRecord
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            BloodType = string.IsNullOrEmpty(bloodType) ? null : bloodType.Trim(),
            Allergies = string.IsNullOrEmpty(allergies) ? null : allergies.Trim(),
            ChronicDiseases = string.IsNullOrEmpty(chronicDiseases) ? null : chronicDiseases.Trim(),
            Medications = string.IsNullOrEmpty(medications) ? null : medications.Trim(),
            EmergencyHealthInfo = string.IsNullOrEmpty(emergencyHealthInfo) ? null : emergencyHealthInfo.Trim(),
            Notes = string.IsNullOrEmpty(notes) ? null : notes.Trim(),
            LastCheckupDate = DateTime.UtcNow,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    public void UpdateBloodType(string? bloodType)
    {
        BloodType = string.IsNullOrEmpty(bloodType) ? null : bloodType.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }
    public void UpdateAllergies(string? allergies)
    {
        Allergies = string.IsNullOrEmpty(allergies) ? null : allergies.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }
    public void UpdateChronicDiseases(string? chronicDiseases)
    {
        ChronicDiseases = string.IsNullOrEmpty(chronicDiseases) ? null : chronicDiseases.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }
    public void UpdateMedications(string? medications)
    {
        Medications = string.IsNullOrEmpty(medications) ? null : medications.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }
    public void UpdateEmergencyHealthInfo(string? emergencyHealthInfo)
    {
        EmergencyHealthInfo = string.IsNullOrEmpty(emergencyHealthInfo) ? null : emergencyHealthInfo.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrEmpty(notes) ? null : notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void ClearAllHealthInfo()
    {
        BloodType = null;
        Allergies = null;
        ChronicDiseases = null;
        Medications = null;
        EmergencyHealthInfo = null;
        Notes = null;
        LastCheckupDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool IsEmpty =>
    string.IsNullOrEmpty(BloodType) &&
    string.IsNullOrEmpty(Allergies) &&
    string.IsNullOrEmpty(ChronicDiseases) &&
    string.IsNullOrEmpty(Medications) &&
    string.IsNullOrEmpty(EmergencyHealthInfo) &&
    string.IsNullOrEmpty(Notes);
}