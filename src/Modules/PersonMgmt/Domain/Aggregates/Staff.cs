using Core.Domain;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.ValueObjects;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// Staff - Personel (Öğretim üyesi, yönetici, vb.) Entity
/// 
/// Özellikleri:
/// - Identity'si var
/// - Mutable
/// - Person Aggregate'ine ait (Child entity)
/// - Personel spesifik bilgilerini içerir
/// </summary>
public class Staff : Entity
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
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Adres bilgisi (ValueObject)
    /// </summary>
    public Address? Address { get; private set; }

    /// <summary>
    /// Acil durum iletişim (ValueObject)
    /// </summary>
    public EmergencyContact? EmergencyContact { get; private set; }

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
    private Staff()
    {
    }

    /// <summary>
    /// Factory method - Yeni personel oluştur
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
            EmployeeNumber = employeeNumber,
            AcademicTitle = academicTitle,
            HireDate = hireDate,
            IsActive = true,
            Address = address,
            EmergencyContact = emergencyContact,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Akademik ünvanı güncelle
    /// </summary>
    public void UpdateAcademicTitle(AcademicTitle newTitle)
    {
        AcademicTitle = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adres bilgisini güncelle
    /// </summary>
    public void UpdateAddress(Address newAddress)
    {
        if (newAddress == null)
            throw new ArgumentNullException(nameof(newAddress));

        Address = newAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Acil iletişi güncelle
    /// </summary>
    public void UpdateEmergencyContact(EmergencyContact newContact)
    {
        if (newContact == null)
            throw new ArgumentNullException(nameof(newContact));

        EmergencyContact = newContact;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Personeli aktifleştir
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Personeli pasifleştir
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        IsActive = false;
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