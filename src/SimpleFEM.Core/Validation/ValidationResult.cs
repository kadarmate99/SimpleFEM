namespace SimpleFEM.Core.Validation;

public sealed class ValidationResult<TErrorCode> where TErrorCode : struct, Enum
{
    public IReadOnlyList<ValidationError<TErrorCode>> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public ValidationResult(IEnumerable<ValidationError<TErrorCode>>? errors = null)
        => Errors = errors?.ToArray() ?? [];
}
