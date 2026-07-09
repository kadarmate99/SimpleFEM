using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Validation.Model;

internal sealed class ModelValidator(IEnumerable<IModelValidationRule> rules)
{
    internal ValidationResult<ModelValidationErrorCode> Validate(FemModel model) =>
        new(rules.SelectMany(rule => rule.Validate(model)));
}
