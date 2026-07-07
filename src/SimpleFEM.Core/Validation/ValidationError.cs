namespace SimpleFEM.Core.Validation;

public record ValidationError<TErrorCode>(
    TErrorCode Code,
    string Message,
    object? Subject = null) where TErrorCode : struct, Enum;
