using Core.Domain;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// 🆕 COMPLETE: Student - Öğrenci Entity
/// 
/// Özellikleri:
/// - Identity'si var
/// - Mutable
/// - Person Aggregate'ine ait (Child entity)
/// - Öğrenci spesifik bilgilerini içerir
/// 
/// Not: Bu entity'nin kendi repository'si yok
/// Person repository aracılığıyla yönetilir
/// </summary>
public class Student : Entity
{
    public Guid PersonId { get; private set; }
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
    /// Genel not ortalaması (CGPA) 0.0 - 4.0
    /// </summary>
    public double CGPA { get; private set; }

    /// <summary>
    /// Dönem not ortalaması (SGPA) 0.0 - 4.0
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
    /// Danışman ID (Staff/Advisor)
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
    /// ✅ FIXED & COMPLETE: Factory method - Yeni öğrenci oluştur
    /// </summary>
    public static Student Create(
        Guid personId,
        string studentNumber,
        EducationLevel educationLevel,
        DateTime enrollmentDate,
        Guid? advisorId = null)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be empty", nameof(studentNumber));

        if (enrollmentDate > DateTime.UtcNow)
            throw new ArgumentException("Enrollment date cannot be in the future", nameof(enrollmentDate));

        return new Student
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            StudentNumber = studentNumber.Trim(),
            EducationLevel = educationLevel,
            CurrentSemester = 1,
            Status = StudentStatus.Active,
            CGPA = 0.0,
            SGPA = 0.0,
            TotalCredits = 0,
            CompletedCredits = 0,
            EnrollmentDate = enrollmentDate,
            GraduationDate = null,
            AdvisorId = advisorId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ==================== STUDENT STATUS METHODS ====================

    /// <summary>
    /// Öğrenci durumunu güncelle
    /// </summary>
    public void UpdateStatus(StudentStatus newStatus)
    {
        // Mezun olduktan sonra aktif olamaz
        if (Status == StudentStatus.Graduated && newStatus != StudentStatus.Graduated)
            throw new InvalidOperationException("Graduated student cannot change status");

        // Çıkarılan öğrenci durumu değiştiremez
        if (Status == StudentStatus.Expelled)
            throw new InvalidOperationException("Expelled student cannot change status");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        // Mezuniyet durumuna geçerse tarihi set et
        if (newStatus == StudentStatus.Graduated && !GraduationDate.HasValue)
            GraduationDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Öğrenciyi askıya al
    /// </summary>
    public void Suspend()
    {
        UpdateStatus(StudentStatus.Suspended);
    }

    /// <summary>
    /// Öğrenciyi pasif yap
    /// </summary>
    public void MakePassive()
    {
        UpdateStatus(StudentStatus.Passive);
    }

    /// <summary>
    /// Öğrenciyi aktif yap
    /// </summary>
    public void MakeActive()
    {
        UpdateStatus(StudentStatus.Active);
    }

    /// <summary>
    /// Öğrenciyi mezun yap
    /// </summary>
    public void Graduate()
    {
        UpdateStatus(StudentStatus.Graduated);
    }

    /// <summary>
    /// Öğrenciyi çıkar
    /// </summary>
    public void Expel()
    {
        UpdateStatus(StudentStatus.Expelled);
    }

    // ==================== ACADEMIC PERFORMANCE ====================

    /// <summary>
    /// CGPA güncelle
    /// </summary>
    public void UpdateCGPA(double cgpa)
    {
        if (cgpa < 0.0 || cgpa > 4.0)
            throw new ArgumentException("CGPA must be between 0.0 and 4.0", nameof(cgpa));

        CGPA = Math.Round(cgpa, 2);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// SGPA (dönem not ortalaması) güncelle
    /// </summary>
    public void UpdateSGPA(double sgpa)
    {
        if (sgpa < 0.0 || sgpa > 4.0)
            throw new ArgumentException("SGPA must be between 0.0 and 4.0", nameof(sgpa));

        SGPA = Math.Round(sgpa, 2);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kredi bilgisi güncelle
    /// </summary>
    public void UpdateCredits(int totalCredits, int completedCredits)
    {
        if (totalCredits < 0)
            throw new ArgumentException("Total credits cannot be negative", nameof(totalCredits));

        if (completedCredits < 0)
            throw new ArgumentException("Completed credits cannot be negative", nameof(completedCredits));

        if (completedCredits > totalCredits)
            throw new ArgumentException("Completed credits cannot exceed total credits", nameof(completedCredits));

        TotalCredits = totalCredits;
        CompletedCredits = completedCredits;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Dönem artır
    /// </summary>
    public void IncrementSemester()
    {
        CurrentSemester++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Danışman ata
    /// </summary>
    public void AssignAdvisor(Guid advisorId)
    {
        if (advisorId == Guid.Empty)
            throw new ArgumentException("Advisor ID cannot be empty", nameof(advisorId));

        AdvisorId = advisorId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Danışman'ı değiştir
    /// </summary>
    public void ChangeAdvisor(Guid newAdvisorId)
    {
        if (newAdvisorId == Guid.Empty)
            throw new ArgumentException("New advisor ID cannot be empty", nameof(newAdvisorId));

        AdvisorId = newAdvisorId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Danışman'ı kaldır
    /// </summary>
    public void RemoveAdvisor()
    {
        AdvisorId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== SOFT DELETE ====================

    /// <summary>
    /// Soft delete - Öğrenciyi sil
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete geri al - Öğrenciyi restore et
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==================== HELPER PROPERTIES ====================

    /// <summary>
    /// Akademik dersleri başarıyla aldı mı? (GPA >= 2.0)
    /// </summary>
    public bool HasPassingGPA => CGPA >= 2.0;

    /// <summary>
    /// Mezun oldu mu?
    /// </summary>
    public bool IsGraduated => Status == StudentStatus.Graduated;

    /// <summary>
    /// Aktif durumda mı?
    /// </summary>
    public bool IsCurrentlyActive => Status == StudentStatus.Active && !IsDeleted;

    /// <summary>
    /// Tamamlanan derslerin yüzdesi
    /// </summary>
    public double CompletionPercentage => TotalCredits > 0
        ? Math.Round((double)CompletedCredits / TotalCredits * 100, 2)
        : 0.0;
}