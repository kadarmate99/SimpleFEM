using SimpleFEM.Core.Domain;
using System.Data;

namespace SimpleFEM.Core.Validation.Model.Rules;

internal sealed class UniqueEntityIdsRule : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
    {
        return CheckDuplicateIds(model.Nodes, n => n.Id, "Node")
             .Concat(CheckDuplicateIds(model.Materials, m => m.Id, "Material"))
             .Concat(CheckDuplicateIds(model.Sections, s => s.Id, "Section"))
             .Concat(CheckDuplicateIds(model.Elements, e => e.Id, "Element"));
    }

    private static IEnumerable<ValidationError<ModelValidationErrorCode>> CheckDuplicateIds<T>(
    IReadOnlyList<T> items,
    Func<T, int> idOf,
    string kind)
    {
        foreach (var group in items.GroupBy(idOf).Where(g => g.Count() > 1))
            yield return new(
                ModelValidationErrorCode.DuplicateIds,
                $"{kind} id {group.Key} is used {group.Count()} times; ids must be unique.",
                group.ToList());
    }
}
