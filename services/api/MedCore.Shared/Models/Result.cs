namespace MedCore.Shared.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, T? value, string? errorMessage, string? errorCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, null, null);
    public static Result<T> Failure(string errorMessage, string errorCode = "INTERNAL_ERROR") 
        => new Result<T>(false, default, errorMessage, errorCode);
}
