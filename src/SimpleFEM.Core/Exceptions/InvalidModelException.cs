using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Validation;
using SimpleFEM.Core.Validation.Model;

namespace SimpleFEM.Core.Exceptions;

/// <summary>
/// Thrown when the analysis was started on an invalid <see cref="FemModel"/>.
/// </summary>
public sealed class InvalidModelException : ArgumentException
{
    public InvalidModelException(
        ValidationResult<ModelValidationErrorCode> validation, 
        FemModel model,
        string paramName)
        : base(BuildMessage(validation), paramName)
    {
        Validation = validation;
        Model = model;
    }

    public ValidationResult<ModelValidationErrorCode> Validation { get; }
    public FemModel Model { get; }

    private static string BuildMessage(ValidationResult<ModelValidationErrorCode> validation)
    {
        ArgumentNullException.ThrowIfNull(validation);
        return $"The system is invalid and the analysis cannot be started: " +
            string.Join(Environment.NewLine, validation.Errors.Select(e => e.Message));
    }
}
