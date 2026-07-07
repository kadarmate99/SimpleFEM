using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Validation.Model;

internal interface IModelValidationRule
{
    IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model);
}
