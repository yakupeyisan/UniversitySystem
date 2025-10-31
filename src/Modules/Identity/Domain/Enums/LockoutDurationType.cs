namespace Identity.Domain.Enums;

/// <summary>
/// Kilitleme süresi tipleri
/// </summary>
public enum LockoutDurationType
{
    Minutes = 1,
    Hours = 2,
    Days = 3,
    Permanent = 4
}