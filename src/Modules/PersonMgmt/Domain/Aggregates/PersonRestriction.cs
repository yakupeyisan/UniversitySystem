using Core.Domain;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// PersonRestriction - Kişi üzerine konulan kısıtlamalar
/// 
/// Özellikleri:
/// - Identity'si var (her kısıtlama unique)
/// - Zaman sınırı olabilir (kalıcı veya geçici)
/// - Severity seviyesi var
/// 
/// Örnekler:
/// - Öğrenci askıya alınması
/// - Mali sorunlar nedeniyle yemekhaneden yasaklama
/// - COVID karantinası
/// </summary>
public class PersonRestriction : Entity
{
    /// <summary>
    /// Kısıtlama türü
    /// </summary>
    public RestrictionType RestrictionType { get; private set; }

    /// <summary>
    /// Kısıtlamanın kapsamı
    /// </summary>
    public RestrictionLevel RestrictionLevel { get; private set; }

    /// <summary>
    /// Kısıtlamayı uygulayanın ID'si (Admin/System)
    /// </summary>
    public Guid AppliedBy { get; private set; }

    /// <summary>
    /// Başlama tarihi
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Bitiş tarihi (null = kalıcı)
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Neden
    /// </summary>
    public string Reason { get; private set; }

    /// <summary>
    /// Ciddiyet seviyesi
    /// </summary>
    public int Severity { get; private set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; }

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
    private PersonRestriction()
    {
    }

    /// <summary>
    /// Factory method - Yeni kısıtlama oluştur
    /// </summary>
    public static PersonRestriction Create(
        RestrictionType restrictionType,
        RestrictionLevel restrictionLevel,
        Guid appliedBy,
        DateTime startDate,
        DateTime? endDate,
        string reason,
        int severity)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));

        if (severity < 1 || severity > 4)
            throw new ArgumentException("Severity must be between 1-4", nameof(severity));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        return new PersonRestriction
        {
            Id = Guid.NewGuid(),
            RestrictionType = restrictionType,
            RestrictionLevel = restrictionLevel,
            AppliedBy = appliedBy,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason,
            Severity = severity,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Kısıtlama hala aktif mi?
    /// </summary>
    public bool IsCurrentlyActive()
    {
        if (!IsActive || IsDeleted)
            return false;

        var now = DateTime.UtcNow;
        if (now < StartDate)
            return false; // Henüz başlamadı

        if (EndDate.HasValue && now > EndDate.Value)
            return false; // Süresi doldu

        return true;
    }

    /// <summary>
    /// Kısıtlamayı sonlandır (early termination)
    /// </summary>
    public void Terminate()
    {
        EndDate = DateTime.UtcNow;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bitiş tarihini uzat
    /// </summary>
    public void ExtendEndDate(DateTime newEndDate)
    {
        if (newEndDate <= DateTime.UtcNow)
            throw new ArgumentException("New end date must be in the future", nameof(newEndDate));

        if (EndDate.HasValue && newEndDate <= EndDate.Value)
            throw new ArgumentException("New end date must be after current end date", nameof(newEndDate));

        EndDate = newEndDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ciddiyet seviyesini güncelle
    /// </summary>
    public void UpdateSeverity(int newSeverity)
    {
        if (newSeverity < 1 || newSeverity > 4)
            throw new ArgumentException("Severity must be between 1-4", nameof(newSeverity));

        Severity = newSeverity;
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