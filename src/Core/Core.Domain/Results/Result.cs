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
    public sealed record SuccessResult : Result;
    public sealed record ErrorResult(string ErrorCode, string ErrorMessage) : Result;

    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is ErrorResult;

    public static Result Success() => new SuccessResult();
    public static Result Failure(string errorCode, string errorMessage) =>
        new ErrorResult(errorCode, errorMessage);
}

/// <summary>
/// Result<T> - Generic version (value döndür)
/// </summary>
public abstract record Result<T>
{
    public sealed record SuccessResult(T Value, string Message) : Result<T>;
    public sealed record ErrorResult(string ErrorCode, string ErrorMessage) : Result<T>;

    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is ErrorResult;
    public T? Value => this is SuccessResult s ? s.Value : default;

    public static Result<T> Success(T value) => new SuccessResult(value, "İşlem başarılı");
    public static Result<T> Success(T value, string message) => new SuccessResult(value, message);
    public static Result<T> Failure(string errorCode, string errorMessage) =>
        new ErrorResult(errorCode, errorMessage);
    public static Result<T> Failure(string errorMessage) =>
        new ErrorResult("", errorMessage);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, TResult> onFailure) =>
        this switch
        {
            SuccessResult success => onSuccess(success.Value),
            ErrorResult failure => onFailure(failure.ErrorCode, failure.ErrorMessage),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}