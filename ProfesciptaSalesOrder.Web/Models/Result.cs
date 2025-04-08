namespace ProfesciptaSalesOrder.Web.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }
    public int StatusCode { get; }

    protected Result(bool isSuccess, T value, string error, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    // Success factory method
    public static Result<T> Success(T value, int statusCode = 200)
    {
        return new Result<T>(true, value, null, statusCode);
    }

    // Failure factory methods
    public static Result<T> Failure(string error, int statusCode = 400)
    {
        return new Result<T>(false, default, error, statusCode);
    }

    public static Result<T> NotFound(string error = "Resource not found")
    {
        return new Result<T>(false, default, error, 404);
    }

    public static Result<T> Unauthorized(string error = "Unauthorized")
    {
        return new Result<T>(false, default, error, 401);
    }
    public static Result<T> InternalServerError(string error = "Internal Server Error")
    {
        return new Result<T>(false, default, error, 500);
    }
}