using Core.Domain;
using Core.Domain.Specifications;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Events;
using PersonMgmt.Domain.ValueObjects;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// 🆕 COMPLETE: Staff - Personel Entity
/// 
/// Özellikleri:
/// - Identity'si var
/// - Mutable
/// - Person Aggregate'ine ait (Child entity)
/// - Personel (öğretim üyesi, yönetici, vb.) spesifik bilgilerini içerir
/// 
/// Not: Bu entity'nin kendi repository'si yok
/// Person repository aracılığıyla yönetilir
/// </summary>
public class Staff : AggregateRoot,ISoftDelete
{
    /// <summary>
    /// Personel numarası (unique)
    /// </summary>
    public string EmployeeNumber { get; private set; }

    /// <summary>
    /// Akademik ünvan
    /// </summary>
    public AcademicTitle AcademicTitle { get; private set; }

    /// <summary>
    /// İşe alma tarihi
    /// </summary>
    public DateTime HireDate { get; private set; }

    /// <summary>
    /// İşten ayrılma tarihi (null = hala çalışıyor)
    /// </summary>
    public DateTime? TerminationDate { get; private set; }

    /// <summary>
    /// Aktif mi? (hala işe alınmış/çalışıyor)
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Adres bilgisi (ValueObject)
    /// </summary>
    public Address? Address { get; private set; }

    /// <summary>
    /// Acil durum iletişi (ValueObject)
    /// </summary>
    public EmergencyContact? EmergencyContact { get; private set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

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
    private Staff()
    {
    }

    /// <summary>
    /// ✅ FIXED & COMPLETE: Factory method - Yeni personel oluştur
    /// </summary>
    public static Staff Create(
        string employeeNumber,
        AcademicTitle academicTitle,
        DateTime hireDate,
        Address? address = null,
        EmergencyContact? emergencyContact = null)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));

        if (hireDate > DateTime.UtcNow)
            throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));

        return new Staff
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = employeeNumber.Trim(),
            AcademicTitle = academicTitle,
            HireDate = hireDate,
            TerminationDate = null,
            IsActive = true,
            Address = address,
            EmergencyContact = emergencyContact,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ==================== STAFF STATUS METHODS ====================

    /// <summary>
    /// Personeli aktif yap
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        TerminationDate = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Personeli pasif yap (işten çıkar)
    /// </summary>
    public void Terminate()
    {
        IsActive = false;
        TerminationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Personeli deaktif yap (geçici olarak)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== ACADEMIC TITLE METHODS ====================

    /// <summary>
    /// Akademik ünvanı güncelle
    /// </summary>
    public void UpdateAcademicTitle(AcademicTitle newTitle)
    {
        if (AcademicTitle == newTitle)
            return;

        AcademicTitle = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Akademik ünvanı yükselt
    /// </summary>
    public void PromoteToNextTitle()
    {
        AcademicTitle = AcademicTitle switch
        {
            AcademicTitle.Assistant => AcademicTitle.ResearchAssistant,
            AcademicTitle.ResearchAssistant => AcademicTitle.Lecturer,
            AcademicTitle.Lecturer => AcademicTitle.AssociateProfessor,
            AcademicTitle.AssociateProfessor => AcademicTitle.Professor,
            AcademicTitle.Professor => AcademicTitle.Professor, // Already at top
            AcademicTitle.Doctor => AcademicTitle.Professor,
            _ => throw new InvalidOperationException($"Unknown academic title: {AcademicTitle}")
        };

        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== ADDRESS METHODS ====================

    /// <summary>
    /// Adres bilgisini güncelle veya ekle
    /// </summary>
    public void UpdateAddress(Address? newAddress)
    {
        Address = newAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adres bilgisini kaldır
    /// </summary>
    public void RemoveAddress()
    {
        Address = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== EMERGENCY CONTACT METHODS ====================

    /// <summary>
    /// Acil durum iletişi bilgisini güncelle veya ekle
    /// </summary>
    public void UpdateEmergencyContact(EmergencyContact? newContact)
    {
        EmergencyContact = newContact;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Acil durum iletişi bilgisini kaldır
    /// </summary>
    public void RemoveEmergencyContact()
    {
        EmergencyContact = null;
        UpdatedAt = DateTime.UtcNow;
    }


    // ==================== HELPER PROPERTIES ====================

    /// <summary>
    /// Hizmet süresi (yıllar)
    /// </summary>
    public int YearsOfService
    {
        get
        {
            var endDate = TerminationDate ?? DateTime.UtcNow;
            return (endDate - HireDate).Days / 365;
        }
    }


    /// <summary>
    /// Profesyonel mi? (Professor veya Associate Professor)
    /// </summary>
    public bool IsProfessional =>
        AcademicTitle == AcademicTitle.Professor ||
        AcademicTitle == AcademicTitle.AssociateProfessor;

    /// <summary>
    /// Yönetimsel pozisyonda mı?
    /// </summary>
    public bool IsInManagement =>
        AcademicTitle == AcademicTitle.Professor ||
        AcademicTitle == AcademicTitle.AssociateProfessor;


    /// <summary>
    /// ✅ NEW: Proper termination with validation
    /// </summary>
    public void Terminate(DateTime terminationDate)
    {
        // Validation
        if (terminationDate < HireDate)
            throw new InvalidOperationException(
                $"Termination date ({terminationDate:yyyy-MM-dd}) cannot be before hire date ({HireDate:yyyy-MM-dd})"
            );

        if (IsDeleted)
            throw new InvalidOperationException(
                "Cannot terminate an already deleted staff member"
            );

        if (!IsActive)
            throw new InvalidOperationException(
                "Staff member is already inactive"
            );

        // Update state
        TerminationDate = terminationDate;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new StaffTerminatedDomainEvent(
            Id,
            EmployeeNumber,
            terminationDate,
            YearsOfService,
            DateTime.UtcNow
        ));
    }

    /// <summary>
    /// ✅ IMPROVED: Rehire terminated staff
    /// </summary>
    public void Rehire(DateTime newHireDate)
    {
        if (!IsDeleted && TerminationDate.HasValue)
        {
            TerminationDate = null;
            IsActive = true;

            // Update hire date if earlier
            if (newHireDate < HireDate)
                HireDate = newHireDate;

            UpdatedAt = DateTime.UtcNow;
        }
    }

    // ==================== SOFT DELETE ====================

    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== HELPER PROPERTIES ====================

    /// <summary>
    /// ✅ NEW: Hali hazırda çalışıyor mu?
    /// </summary>
    public bool IsCurrentlyEmployed =>
        IsActive &&
        !IsDeleted &&
        (TerminationDate == null || TerminationDate > DateTime.UtcNow);

    /// <summary>
    /// Complete profile check
    /// </summary>
    public bool HasCompleteProfile =>
        Address != null &&
        EmergencyContact != null;
}