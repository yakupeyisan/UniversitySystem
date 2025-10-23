using Core.Domain;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Aggregates;

/// <summary>
/// 🆕 COMPLETE: PersonRestriction - Kişi üzerine konulan kısıtlamalar Entity
/// 
/// Özellikleri:
/// - Identity'si var (her kısıtlama unique)
/// - Zaman sınırı olabilir (kalıcı veya geçici)
/// - Severity seviyesi var
/// - Person Aggregate'ine ait (Child entity)
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
    /// Kısıtlamanın kapsamı (Genel, Yemekhanede, Tüm tesislerde, vb.)
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
    /// Ciddiyet seviyesi (1-10 arası)
    /// </summary>
    public int Severity { get; private set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; private set; }

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
        ValidateRestrictionData(reason, severity, startDate, endDate);

        return new PersonRestriction
        {
            Id = Guid.NewGuid(),
            RestrictionType = restrictionType,
            RestrictionLevel = restrictionLevel,
            AppliedBy = appliedBy,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason.Trim(),
            Severity = severity,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ==================== BUSINESS METHODS ====================

    /// <summary>
    /// ✅ FIXED: Kısıtlama şu anda aktif mi?
    /// Başlama tarihinden sonra ve (bitiş tarihi yoksa kalıcı veya bitiş tarihinden önce) ise aktif
    /// </summary>
    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;

        // Silinmişse aktif değil
        if (IsDeleted)
            return false;

        // Aktif flag'ı false ise
        if (!IsActive)
            return false;

        // Henüz başlanmamış ise
        if (now < StartDate)
            return false;

        // Süresi dolmuş ise (EndDate varsa)
        if (EndDate.HasValue && now > EndDate.Value)
            return false;

        // Diğer tüm durumlarda aktif
        return true;
    }

    /// <summary>
    /// Kısıtlamayı deaktif et
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kısıtlamayı reaktif et
    /// </summary>
    public void Reactivate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kısıtlamanın bitiş tarihini uzat
    /// </summary>
    public void ExtendEndDate(DateTime newEndDate)
    {
        if (newEndDate <= EndDate)
            throw new ArgumentException("New end date must be after current end date", nameof(newEndDate));

        EndDate = newEndDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete - Kısıtlamayı sil
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete geri al - Kısıtlamayı restore et
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        // IsActive önceki durumuna bağlı olarak korunur
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kısıtlamanın kalan gün sayısı
    /// Null = kalıcı (sonsuz)
    /// </summary>
    public int? RemainingDays
    {
        get
        {
            if (!EndDate.HasValue || IsDeleted)
                return null;

            var now = DateTime.UtcNow;
            if (now > EndDate.Value)
                return 0;

            return (int)(EndDate.Value - now).TotalDays;
        }
    }

    /// <summary>
    /// Kısıtlamanın süre biteli mi?
    /// </summary>
    public bool IsExpired
    {
        get
        {
            if (!EndDate.HasValue)
                return false;

            return DateTime.UtcNow > EndDate.Value;
        }
    }

    /// <summary>
    /// Kısıtlama kalıcı mı? (bitiş tarihi yok)
    /// </summary>
    public bool IsPermanent => !EndDate.HasValue;

    // ==================== VALIDATION ====================

    private static void ValidateRestrictionData(
        string reason,
        int severity,
        DateTime startDate,
        DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Restriction reason cannot be empty", nameof(reason));

        if (reason.Length < 10)
            throw new ArgumentException("Restriction reason must be at least 10 characters", nameof(reason));

        if (severity < 1 || severity > 10)
            throw new ArgumentException("Severity must be between 1 and 10", nameof(severity));

        if (startDate > DateTime.UtcNow)
            throw new ArgumentException("Start date cannot be in the future", nameof(startDate));

        if (endDate.HasValue && endDate.Value <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));
    }
}