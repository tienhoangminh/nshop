namespace nShop.Core.SeedWork.Results;

public class ResultError(
    string errorCode,
    string errorMessage,
    string propertyName = "",
    Exception? exception = default)
{
    public string ErrorCode { get; } = errorCode;
    public string PropertyName { get; } = propertyName;
    public string ErrorMessage { get; } = errorMessage;
    public Exception? Exception { get; } = exception;
}

public class ValidationError(string propertyName, string errorMessage = "")
    : ResultError("Validation", errorMessage, propertyName)
{
}