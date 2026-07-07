namespace SimpleFEM.Core.Validation;

public sealed class ValidationResult<TErrorCode> where TErrorCode : struct, Enum
{
    private readonly List<ValidationError<TErrorCode>> errors = new();
    public IReadOnlyList<ValidationError<TErrorCode>> Errors => errors;

    public bool IsValid => !errors.Any();

    public void Add(ValidationError<TErrorCode> error) => errors.Add(error);
    public void AddRange(IEnumerable<ValidationError<TErrorCode>> errors) => this.errors.AddRange(errors);
}
