using SimpleFEM.Core.Domain;
using System.Data;

namespace SimpleFEM.Core.Validation.Model.Rules;

internal sealed class ReferenceIntegrityRule : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
    {
        var nodeIds = model.Nodes.Select(n => n.Id).ToHashSet();
        var materialIds = model.Materials.Select(m => m.Id).ToHashSet();
        var sectionIds = model.Sections.Select(s => s.Id).ToHashSet();

        foreach (var e in model.Elements)
        {
            foreach (var error in CheckRefs(e, e.NodeIds, nodeIds, "node", "Element", e.Id))
                yield return error;
            foreach (var error in CheckRefs(e, [e.MaterialId], materialIds, "material", "Element", e.Id))
                yield return error;
            foreach (var error in CheckRefs(e, [e.SectionId], sectionIds, "section", "Element", e.Id))
                yield return error;
        }

        foreach (var s in model.Supports)
            foreach (var error in CheckRefs(s, [s.NodeId], nodeIds, "node", "Support", s.NodeId))
                yield return error;

        foreach (var l in model.Loads)
            foreach (var error in CheckRefs(l, [l.NodeId], nodeIds, "node", "Nodal load", l.NodeId))
                yield return error;
    }

    private static IEnumerable<ValidationError<ModelValidationErrorCode>> CheckRefs(
        object subject,
        IEnumerable<int> referencedIds,
        HashSet<int> existingIds,
        string targetKind,
        string sourceKind,
        int sourceId)
    {
        foreach (var id in referencedIds)
            if (!existingIds.Contains(id))
                yield return new(
                    ModelValidationErrorCode.UnknownReference,
                    $"{sourceKind} {sourceId} references unknown {targetKind} {id}.",
                    subject);
    }
}
