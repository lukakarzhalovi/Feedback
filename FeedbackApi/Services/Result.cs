namespace FeedbackApi.Services;

public record Result(bool IsSuccess, List<string> Errors)
{
    public static Result Success() => new(true, []);
    public static Result Failure(List<string> errors) => new(false, errors);
}

public record Result<T>(bool IsSuccess, T? Value, List<string> Errors)
{
    public static Result<T> Success(T value) => new(true, value, []);
    public static Result<T> Failure(List<string> errors) => new(false, default, errors);
    public static Result<T> Failure(string error) => new(false, default, [error]);
}

