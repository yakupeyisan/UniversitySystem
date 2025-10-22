using Core.Domain;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// Student - Öğrenci bilgileri Entity
/// 
/// Özellikleri:
/// - Identity'si var (Id)
/// - Mutable
/// - Person Aggregate'ine ait (Child entity)
/// - Öğrenci spesifik bilgilerini içerir
/// 
/// Not: Bu entity'nin kendi repository'si yok
/// Person repository aracılığıyla yönetilir
/// </summary>
public class Student : Entity
{
    /// <summary>
    /// Öğrenci numarası (unique)
    /// </summary>
    public string StudentNumber { get; private set; }

    /// <summary>
    /// Eğitim seviyesi
    /// </summary>
    public EducationLevel EducationLevel { get; private set; }

    /// <summary>
    /// Mevcut dönem
    /// </summary>
    public int CurrentSemester { get; private set; }

    /// <summary>
    /// Öğrenci durumu
    /// </summary>
    public StudentStatus Status { get; private set; }

    /// <summary>
    /// Genel not ortalaması (CGPA)
    /// </summary>
    public double CGPA { get; private set; }

    /// <summary>
    /// Dönem not ortalaması (SGPA)
    /// </summary>
    public double SGPA { get; private set; }

    /// <summary>
    /// Toplam kredi
    /// </summary>
    public int TotalCredits { get; private set; }

    /// <summary>
    /// Tamamlanan kredi
    /// </summary>
    public int CompletedCredits { get; private set; }

    /// <summary>
    /// Kayıt tarihi
    /// </summary>
    public DateTime EnrollmentDate { get; private set; }

    /// <summary>
    /// Mezuniyet tarihi (nullable)
    /// </summary>
    public DateTime? GraduationDate { get; private set; }

    /// <summary>
    /// Danışman ID (Staff)
    /// </summary>
    public Guid? AdvisorId { get; private set; }

    /// <summary>
    /// Soft delete flag
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
    /// Private constructor - Factory method üzerinden oluştur
    /// </summary>
    private Student()
    {
    }

    /// <summary>
    /// Factory method - Yeni öğrenci oluştur
    /// </summary>
    public static Student Create(
        string studentNumber,
        EducationLevel educationLevel,
        DateTime enrollmentDate,
        Guid? advisorId = null)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be empty", nameof(studentNumber));

        return new Student
        {
            Id = Guid.NewGuid(),
            StudentNumber = studentNumber,
            EducationLevel = educationLevel,
            CurrentSemester = 1,
            Status = StudentStatus.Active,
            CGPA = 0,
            SGPA = 0,
            TotalCredits = 0,
            CompletedCredits = 0,
            EnrollmentDate = enrollmentDate,
            AdvisorId = advisorId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Dönem ilerlet
    /// </summary>
    public void AdvanceSemester()
    {
        if (Status != StudentStatus.Active)
            throw new InvalidOperationException("Only active students can advance semester");

        CurrentSemester++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// CGPA güncelle
    /// </summary>
    public void UpdateCGPA(double newCGPA)
    {
        if (newCGPA < 0 || newCGPA > 4.0)
            throw new ArgumentException("CGPA must be between 0 and 4.0", nameof(newCGPA));

        CGPA = newCGPA;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// SGPA güncelle
    /// </summary>
    public void UpdateSGPA(double newSGPA)
    {
        if (newSGPA < 0 || newSGPA > 4.0)
            throw new ArgumentException("SGPA must be between 0 and 4.0", nameof(newSGPA));

        SGPA = newSGPA;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kredi bilgisini güncelle
    /// </summary>
    public void UpdateCredits(int totalCredits, int completedCredits)
    {
        if (totalCredits < 0 || completedCredits < 0)
            throw new ArgumentException("Credits cannot be negative");

        if (completedCredits > totalCredits)
            throw new ArgumentException("Completed credits cannot exceed total credits");

        TotalCredits = totalCredits;
        CompletedCredits = completedCredits;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Danışman ata
    /// </summary>
    public void AssignAdvisor(Guid advisorId)
    {
        AdvisorId = advisorId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Öğrenci durumunu değiştir
    /// </summary>
    public void UpdateStatus(StudentStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == StudentStatus.Graduated && GraduationDate == null)
        {
            GraduationDate = DateTime.UtcNow;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete - Öğrenciyi sil
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