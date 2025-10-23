using Core.Domain;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// 🆕 COMPLETE: HealthRecord - Sağlık kaydı Entity
/// 
/// Özellikleri:
/// - Identity'si var
/// - Mutable
/// - Person Aggregate'ine ait (Child entity)
/// - Person'un sağlık bilgilerini içerir
/// - ONE-TO-ONE relationship
/// </summary>
public class HealthRecord : Entity
{
    /// <summary>
    /// Kan grubu
    /// </summary>
    public string? BloodType { get; private set; }

    /// <summary>
    /// Alerji bilgisi
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
    /// Notlar / Açıklamalar
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Son kontrol tarihi
    /// </summary>
    public DateTime? LastCheckupDate { get; private set; }

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
    /// Factory method - Yeni sağlık kaydı oluştur
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

    // ==================== UPDATE METHODS ====================

    /// <summary>
    /// Kan grubu güncelle
    /// </summary>
    public void UpdateBloodType(string? bloodType)
    {
        BloodType = string.IsNullOrEmpty(bloodType) ? null : bloodType.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Alerji bilgisi güncelle
    /// </summary>
    public void UpdateAllergies(string? allergies)
    {
        Allergies = string.IsNullOrEmpty(allergies) ? null : allergies.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Kronik hastalıklar güncelle
    /// </summary>
    public void UpdateChronicDiseases(string? chronicDiseases)
    {
        ChronicDiseases = string.IsNullOrEmpty(chronicDiseases) ? null : chronicDiseases.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Kullanılan ilaçlar güncelle
    /// </summary>
    public void UpdateMedications(string? medications)
    {
        Medications = string.IsNullOrEmpty(medications) ? null : medications.Trim();
        UpdatedAt = DateTime.UtcNow;
        LastCheckupDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Acil durum sağlık bilgisi güncelle
    /// </summary>
    public void UpdateEmergencyHealthInfo(string? emergencyHealthInfo)
    {
        EmergencyHealthInfo = string.IsNullOrEmpty(emergencyHealthInfo) ? null : emergencyHealthInfo.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Notlar güncelle
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrEmpty(notes) ? null : notes.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete - Sağlık kaydını sil
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete geri al - Sağlık kaydını restore et
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Tüm sağlık bilgisini temizle
    /// </summary>
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

    /// <summary>
    /// Sağlık kaydı boş mu? (tüm bilgiler null ise)
    /// </summary>
    public bool IsEmpty =>
        string.IsNullOrEmpty(BloodType) &&
        string.IsNullOrEmpty(Allergies) &&
        string.IsNullOrEmpty(ChronicDiseases) &&
        string.IsNullOrEmpty(Medications) &&
        string.IsNullOrEmpty(EmergencyHealthInfo) &&
        string.IsNullOrEmpty(Notes);
}


