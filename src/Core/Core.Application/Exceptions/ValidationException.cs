using FluentValidation.Results;

namespace Core.Application.Exceptions;

/// <summary>
/// ValidationException - Validation error exception
/// 
/// Sorumluluğu:
/// - FluentValidation'ın ValidationFailure'larını wrap etme
/// - Detailed validation errors sağlama
/// 
/// Kullanım:
/// var failures = new List<ValidationFailure> { ... };
/// throw new ValidationException(failures);
/// 
/// Details property'si:
/// {
///   "errors": {
///     "Email": ["Invalid email format"],
///     "Age": ["Must be 18 or older"]
///   }
/// }
/// </summary>
public class ValidationException : ApplicationException
{
    public override string ErrorCode => "VALIDATION_ERROR";
    public override int StatusCode => 400; // Bad Request

    /// <summary>
    /// Validation errors (field -> messages)
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public override object? Details => new { Errors };

    /// <summary>
    /// ValidationFailure'lardan ValidationException yap
    /// </summary>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(BuildMessage(failures))
    {
        // FluentValidation failures'ları field -> messages şeklinde group et
        var groupedFailures = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());

        Errors = groupedFailures.AsReadOnly();
    }

    /// <summary>
    /// Error message oluştur
    /// </summary>
    private static string BuildMessage(IEnumerable<ValidationFailure> failures)
    {
        var failureList = failures.ToList();

        if (!failureList.Any())
            return "One or more validation failures have occurred.";

        var failureCount = failureList.Count;
        return failureCount == 1
            ? $"One validation error occurred: {failureList[0].ErrorMessage}"
            : $"{failureCount} validation errors occurred.";
    }
}