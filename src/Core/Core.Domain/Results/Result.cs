namespace Core.Domain.Results;

public abstract record Result
{
    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is ErrorResult;

    public static Result Success()
    {
        return new SuccessResult();
    }

    public static Result Failure(string errorCode, string errorMessage)
    {
        return new ErrorResult(errorCode, errorMessage);
    }

    public sealed record SuccessResult : Result;

    public sealed record ErrorResult(string ErrorCode, string ErrorMessage) : Result;
}

public abstract record Result<T>
{
    public bool IsSuccess => this is SuccessResult;
    public bool IsFailure => this is ErrorResult;
    public T? Value => this is SuccessResult s ? s.Value : default;

    public static Result<T> Success(T value)
    {
        return new SuccessResult(value, "İşlem başarılı");
    }

    public static Result<T> Success(T value, string message)
    {
        return new SuccessResult(value, message);
    }

    public static Result<T> Failure(string errorCode, string errorMessage)
    {
        return new ErrorResult(errorCode, errorMessage);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new ErrorResult("", errorMessage);
    }

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, TResult> onFailure)
    {
        return this switch
        {
            SuccessResult success => onSuccess(success.Value),
            ErrorResult failure => onFailure(failure.ErrorCode, failure.ErrorMessage),
            _ => throw new InvalidOperationException("Unknown result type")
        };
    }

    public sealed record SuccessResult(T Value, string Message) : Result<T>;

    public sealed record ErrorResult(string ErrorCode, string ErrorMessage) : Result<T>;
}