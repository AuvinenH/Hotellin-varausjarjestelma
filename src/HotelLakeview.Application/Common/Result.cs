namespace HotelLakeview.Application.Common;

public class Result
{
    protected Result(bool isSuccess, ResultError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultError? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(ResultError error) => new(false, error);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, null)
    {
        _value = value;
    }

    private Result(ResultError error)
        : base(false, error)
    {
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value for a failed result.");

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(ResultError error) => new(error);
}
