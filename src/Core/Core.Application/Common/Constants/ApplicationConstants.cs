namespace Core.Application.Common.Constants;

/// <summary>
/// ApplicationConstants - Uygulama genelinde sabit değerler
/// 
/// Kullanım:
/// if (pageSize > ApplicationConstants.MaxPageSize)
///     throw new ValidationException("Page size too large");
/// 
/// var cacheKey = $"{ApplicationConstants.CacheKeyPrefix}:person:{personId}";
/// </summary>
public static class ApplicationConstants
{
    // ==================== PAGINATION ====================
    /// <summary>
    /// Maksimum sayfa boyutu
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Varsayılan sayfa boyutu
    /// </summary>
    public const int DefaultPageSize = 10;

    // ==================== CACHE ====================
    /// <summary>
    /// Cache key prefix
    /// </summary>
    public const string CacheKeyPrefix = "app";

    /// <summary>
    /// Kişi cache key'i
    /// </summary>
    public const string PersonCacheKey = "app:person";

    /// <summary>
    /// Default cache duration (minutes)
    /// </summary>
    public const int DefaultCacheDurationMinutes = 30;

    // ==================== VALIDATION ====================
    /// <summary>
    /// Min name length
    /// </summary>
    public const int MinNameLength = 2;

    /// <summary>
    /// Max name length
    /// </summary>
    public const int MaxNameLength = 100;

    /// <summary>
    /// Min email length
    /// </summary>
    public const int MinEmailLength = 5;

    /// <summary>
    /// Max email length
    /// </summary>
    public const int MaxEmailLength = 255;

    // ==================== RETRY ====================
    /// <summary>
    /// Default retry count
    /// </summary>
    public const int DefaultRetryCount = 3;

    /// <summary>
    /// Default retry delay (ms)
    /// </summary>
    public const int DefaultRetryDelayMs = 1000;
}