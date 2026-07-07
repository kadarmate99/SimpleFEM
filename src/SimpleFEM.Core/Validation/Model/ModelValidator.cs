using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Validation.Model;

internal sealed class ModelValidator(IEnumerable<IModelValidationRule> rules)
{
    internal ValidationResult<ModelValidationErrorCode> Validate(FemModel model)
    {
        var result = new ValidationResult<ModelValidationErrorCode>();
        foreach (var rule in rules)
            result.AddRange(rule.Validate(model));
        return result;
    }
}
