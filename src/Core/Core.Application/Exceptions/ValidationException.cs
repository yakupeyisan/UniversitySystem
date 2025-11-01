using FluentValidation.Results;
namespace Core.Application.Exceptions;
public class ValidationException : ApplicationException
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base(BuildMessage(failures))
    {
        var groupedFailures = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());
        Errors = groupedFailures.AsReadOnly();
    }
    public override string ErrorCode => "VALIDATION_ERROR";
    public override int StatusCode => 400;
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public override object? Details => new { Errors };
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