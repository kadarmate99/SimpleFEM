using SimpleFEM.Core.Exceptions;
using SimpleFEM.Core.Validation;
using SimpleFEM.Core.Validation.Result;

namespace SimpleFEM.Core.Results;

/// <summary>
/// Carries the <see cref="AnalysisResult"/> and post-run result validation. 
/// Check <see cref="IsValid"/> or call <see cref="EnsureValid"/> to enforce a valid result.
/// </summary>
public sealed class AnalysisOutcome
{
    internal AnalysisOutcome(AnalysisResult result, ValidationResult<ResultValidationErrorCode> validation)
    {
        Result = result;
        Validation = validation;
    }

    public AnalysisResult Result { get; }

    public ValidationResult<ResultValidationErrorCode> Validation { get; }

    public bool IsValid => Validation.IsValid;

    public AnalysisResult EnsureValid()
        => IsValid
            ? Result
            : throw new InvalidAnalysisResultException(Validation, Result);
}