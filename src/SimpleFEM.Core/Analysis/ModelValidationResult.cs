namespace SimpleFEM.Core.Analysis;

public record ModelValidationResult(bool IsValid, IReadOnlyList<ModelValidationError> Errors);
