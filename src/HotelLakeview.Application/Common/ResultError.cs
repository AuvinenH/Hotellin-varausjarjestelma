namespace HotelLakeview.Application.Common;

public enum ResultErrorType
{
    Validation,
    NotFound,
    Conflict,
    InvalidOperation,
}

public sealed record ResultError(
    ResultErrorType Type,
    string Code,
    string Message)
{
    public static ResultError Validation(string code, string message) => new(ResultErrorType.Validation, code, message);

    public static ResultError NotFound(string code, string message) => new(ResultErrorType.NotFound, code, message);

    public static ResultError Conflict(string code, string message) => new(ResultErrorType.Conflict, code, message);

    public static ResultError InvalidOperation(string code, string message) => new(ResultErrorType.InvalidOperation, code, message);
}
