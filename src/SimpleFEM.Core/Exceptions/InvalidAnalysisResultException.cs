using SimpleFEM.Core.Validation;
using SimpleFEM.Core.Results;
using SimpleFEM.Core.Validation.Result;

namespace SimpleFEM.Core.Exceptions;

/// <summary>Thrown by <see cref="AnalysisOutcome.EnsureValid"/> if the result is invalid.</summary>
public sealed class InvalidAnalysisResultException : Exception
{
    public InvalidAnalysisResultException(
        ValidationResult<ResultValidationErrorCode> validation,
        AnalysisResult result)
        : base(BuildMessage(validation))
    {
        Validation = validation;
        Result = result;
    }

    public ValidationResult<ResultValidationErrorCode> Validation { get; }

    /// <summary>The failing solution, preserved. Never discarded.</summary>
    public AnalysisResult Result { get; }

    private static string BuildMessage(ValidationResult<ResultValidationErrorCode> validation)
    {
        ArgumentNullException.ThrowIfNull(validation);
        return $"The solution failed validation: " +
             string.Join(Environment.NewLine, validation.Errors.Select(e => e.Message));
    }
}
