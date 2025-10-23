namespace Core.Domain.Filtering;

/// <summary>
/// Tek bir filter expression'ı temsil eder
/// 
/// Örnek: "price|gt|100" → FilterExpression("price", GreaterThan, ["100"])
/// </summary>
public class FilterExpression
{
    /// <summary>
    /// Filter uygulanacak property adı (nested: "address.city" destekli)
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Filter operatörü
    /// </summary>
    public FilterOperator Operator { get; set; }

    /// <summary>
    /// Filter değerleri (Between, In için multiple values)
    /// </summary>
    public List<string> Values { get; set; }

    /// <summary>
    /// Null/NotNull operatörleri için (value gerektirmez)
    /// </summary>
    public FilterExpression(string propertyName, FilterOperator op)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        Operator = op;
        Values = new List<string>();
    }

    /// <summary>
    /// Diğer operatörler için (1+ value)
    /// </summary>
    public FilterExpression(string propertyName, FilterOperator op, params string[] values)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        Operator = op;
        Values = values?.ToList() ?? new List<string>();

        // Validation
        if (Values.Count == 0 && op != FilterOperator.IsNull && op != FilterOperator.IsNotNull)
            throw new ArgumentException($"Operator '{op}' için en az 1 value gerekli", nameof(values));
    }

    public override string ToString()
    {
        return $"{PropertyName}|{Operator}|{string.Join(",", Values)}";
    }
}