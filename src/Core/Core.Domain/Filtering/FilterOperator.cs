namespace Core.Domain.Filtering;

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