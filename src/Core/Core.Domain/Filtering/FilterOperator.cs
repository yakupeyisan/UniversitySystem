namespace Core.Domain.Filtering;

/// <summary>
/// Filter operatörü enum'ı
/// 
/// Desteklenen operatörler:
/// - Equals, NotEquals (eq, neq)
/// - Comparison: GreaterThan, GreaterOrEqual, LessThan, LessOrEqual
/// - String: Contains, StartsWith, EndsWith
/// - Range: Between
/// - List: In
/// - Null check: IsNull, IsNotNull
/// </summary>
public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterOrEqual,
    LessThan,
    LessOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    Between,
    In,
    IsNull,
    IsNotNull
}