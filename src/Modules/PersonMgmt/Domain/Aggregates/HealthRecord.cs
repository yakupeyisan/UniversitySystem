using Core.Domain;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// HealthRecord - Sağlık bilgileri Entity
/// 
/// Özellikleri:
/// - Identity'si var
/// - Person Aggregate'ine ait (Child entity)
/// - Tıbbi bilgileri saklar
/// 
/// Not: Hassas veri - Encryption/masking gerekebilir
/// </summary>
public class HealthRecord : Entity
{
    /// <summary>
    /// Kan grubu
    /// </summary>
    public string? BloodType { get; private set; }

    /// <summary>
    /// Alerjiler
    /// </summary>
    public string? Allergies { get; private set; }

    /// <summary>
    /// Kronik hastalıklar
    /// </summary>
    public string? ChronicDiseases { get; private set; }

    /// <summary>
    /// Kullanılan ilaçlar
    /// </summary>
    public string? Medications { get; private set; }

    /// <summary>
    /// Acil durum sağlık bilgisi
    /// </summary>
    public string? EmergencyHealthInfo { get; private set; }

    /// <summary>
    /// Son muayene tarihi
    /// </summary>
    public DateTime? LastCheckup { get; private set; }

    /// <summary>
    /// Notlar
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Soft delete
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Güncelleme tarihi
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private HealthRecord()
    {
    }

    /// <summary>
    /// Factory method - Sağlık kaydı oluştur
    /// </summary>
    public static HealthRecord Create(
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
            BloodType = bloodType,
            Allergies = allergies,
            ChronicDiseases = chronicDiseases,
            Medications = medications,
            EmergencyHealthInfo = emergencyHealthInfo,
            Notes = notes,
            LastCheckup = null,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Kan grubu güncelle
    /// </summary>
    public void UpdateBloodType(string bloodType)
    {
        if (!string.IsNullOrWhiteSpace(bloodType))
        {
            // Kan grubu validation (A, B, AB, O + Rh)
            var validBloodTypes = new[] { "A", "B", "AB", "O" };
            var cleanType = bloodType.Replace("+", "").Replace("-", "");

            if (!validBloodTypes.Contains(cleanType))
                throw new ArgumentException("Invalid blood type", nameof(bloodType));
        }

        BloodType = bloodType;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Alerjileri güncelle
    /// </summary>
    public void UpdateAllergies(string? allergies)
    {
        Allergies = allergies;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kronik hastalıkları güncelle
    /// </summary>
    public void UpdateChronicDiseases(string? chronicDiseases)
    {
        ChronicDiseases = chronicDiseases;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// İlaçları güncelle
    /// </summary>
    public void UpdateMedications(string? medications)
    {
        Medications = medications;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Acil durum bilgisini güncelle
    /// </summary>
    public void UpdateEmergencyHealthInfo(string? emergencyHealthInfo)
    {
        EmergencyHealthInfo = emergencyHealthInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Muayene kaydını güncelle
    /// </summary>
    public void RecordCheckup(DateTime checkupDate)
    {
        if (checkupDate > DateTime.UtcNow)
            throw new ArgumentException("Checkup date cannot be in the future", nameof(checkupDate));

        LastCheckup = checkupDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Notları güncelle
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete geri al
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
}