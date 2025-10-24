namespace Core.Domain.Filtering;
public class FilterExpression
{
    public string PropertyName { get; set; }
    public FilterOperator Operator { get; set; }
    public List<string> Values { get; set; }
    public FilterExpression(string propertyName, FilterOperator op)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        Operator = op;
        Values = new List<string>();
    }
    public FilterExpression(string propertyName, FilterOperator op, params string[] values)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        Operator = op;
        Values = values?.ToList() ?? new List<string>();
        if (Values.Count == 0 && op != FilterOperator.IsNull && op != FilterOperator.IsNotNull)
            throw new ArgumentException($"Operator '{op}' için en az 1 value gerekli", nameof(values));
    }
    public override string ToString()
    {
        return $"{PropertyName}|{Operator}|{string.Join(",", Values)}";
    }
}