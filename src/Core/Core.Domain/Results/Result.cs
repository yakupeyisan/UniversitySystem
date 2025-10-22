namespace Core.Domain.Results;

/// <summary>
/// Result Pattern - Exception yerine Result döndür
/// 
/// Avantajları:
/// - Explicit error handling
/// - No exception overhead
/// - Railway-oriented programming
/// - Functional programming style
/// 
/// Kullanım:
/// var result = await CreatePersonAsync(request);
/// if (result.IsFailure)
/// {
///     return BadRequest(result.Error);
/// }
/// var person = result.Value;
/// </summary>
public abstract record Result
{
    public sealed record Success : Result;
    public sealed record Failure(string ErrorCode, string ErrorMessage) : Result;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result Ok() => new Success();
    public static Result Fail(string errorCode, string errorMessage) =>
        new Failure(errorCode, errorMessage);
}

/// <summary>
/// Result<T> - Generic version (value döndür)
/// </summary>
public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string ErrorCode, string ErrorMessage) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result<T> Ok(T value) => new Success(value);
    public static Result<T> Fail(string errorCode, string errorMessage) =>
        new Failure(errorCode, errorMessage);
    public static Result<T> Fail(string errorMessage) =>
        new Failure("errors.internal_error", errorMessage);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, TResult> onFailure) =>
        this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.ErrorCode, failure.ErrorMessage),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}