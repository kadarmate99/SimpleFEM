using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Validation.Model.Rules;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Materials and sections are technically not needed for a complete model (eg a model of only springs...), 
/// they are only dependencies of specific element types.
/// </remarks>
internal sealed class ModelIsPopulatedRule : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
    {
        if (model.Nodes.Count == 0)
            yield return new(
                ModelValidationErrorCode.EmptyModel,
                "The model has no nodes.");

        if (model.Elements.Count == 0)
            yield return new(
                ModelValidationErrorCode.EmptyModel,
                "The model has no elements.");

        if (model.Supports.Count == 0)
            yield return new(
                ModelValidationErrorCode.EmptyModel,
                "The model has no supports.");

        if (model.Loads.Count == 0)
            yield return new(
                ModelValidationErrorCode.EmptyModel,
                "The model has no loads.");
    }
}
