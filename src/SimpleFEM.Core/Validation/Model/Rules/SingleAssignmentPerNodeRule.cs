using SimpleFEM.Core.Domain;
using System.Data;

namespace SimpleFEM.Core.Validation.Model.Rules;

internal sealed class SingleAssignmentPerNodeRule : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
        => CheckAtMostOnePerNode(model.Supports, s => s.NodeId, "support")
            .Concat(CheckAtMostOnePerNode(model.Loads, l => l.NodeId, "load"));

    private static IEnumerable<ValidationError<ModelValidationErrorCode>> CheckAtMostOnePerNode<T>(
        IReadOnlyList<T> items, Func<T, int> nodeIdOf, string kind)
    {
        foreach (var group in items.GroupBy(nodeIdOf).Where(g => g.Count() > 1))
            yield return new(
                ModelValidationErrorCode.DuplicateNodeAssignment,
                $"Node {group.Key} has {group.Count()} {kind}s; only zero or one is allowed.",
                group.ToList());
    }
}
