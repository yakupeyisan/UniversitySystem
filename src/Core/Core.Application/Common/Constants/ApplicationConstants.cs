namespace Core.Application.Common.Constants;
public static class ApplicationConstants
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 10;
    public const string CacheKeyPrefix = "app";
    public const string PersonCacheKey = "app:person";
    public const int DefaultCacheDurationMinutes = 30;
    public const int MinNameLength = 2;
    public const int MaxNameLength = 100;
    public const int MinEmailLength = 5;
    public const int MaxEmailLength = 255;
    public const int DefaultRetryCount = 3;
    public const int DefaultRetryDelayMs = 1000;
}