using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Preprocessing;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.PostProcessing;

internal sealed class PostProcessor
{
    internal AnalysisResult Recover(
        FemModel model,
        GlobalDofIndexMap dofMap,
        GlobalSystem system,
        Vector<double> u)
    {
        var nodalDisplacements = RecoverNodalDisplacements(u, dofMap);
        var reactions = RecoverReactions(
            u, system.K, system.F, dofMap, model.GetRestrainedDofs().ToList());
        var elementInternalForces = RecoverElementInternalForces(u, model, dofMap);

        return new AnalysisResult(nodalDisplacements, reactions, elementInternalForces);
    }

    private List<NodalDisplacementResult> RecoverNodalDisplacements(Vector<double> u, GlobalDofIndexMap dofMap)
    {
        return BuildNodalResults(
            u,
            dofMap,
            // calc displacements for all active nodes.
            // u vector can contain only active nodes, so no filtering is needed
            includeNode: (int nodeId) => true,
            createResult: (int nodeId, double ux, double uy, double rz)
                            => new NodalDisplacementResult(nodeId, ux, uy, rz));
    }

    private List<ReactionResult> RecoverReactions(
        Vector<double> globalDisplacements,
        Matrix<double> globalStiffness,
        Vector<double> globalLoads,
        GlobalDofIndexMap dofMap,
        IReadOnlyList<RestrainedDof> restrainedDofs)
    {
        var globalReactions = globalStiffness * globalDisplacements - globalLoads;
        var restrainedNodeIds = restrainedDofs.Select(r => r.Dof.NodeId).ToHashSet();

        return BuildNodalResults(
            globalReactions,
            dofMap,
            includeNode: (nodeId) => (restrainedNodeIds.Contains(nodeId)),
            createResult: (nodeId, rx, ry, mz)
                            => new ReactionResult(nodeId, rx, ry, mz));
    }

    private List<ElementInternalForceResult> RecoverElementInternalForces(
        Vector<double> globalDisplacements,
        FemModel model,
        GlobalDofIndexMap dofMap)
    {

        var elementForces = new List<ElementInternalForceResult>();

        foreach (var element in model.Elements)
        {
            var nodeI = model.GetNode(element.NodeI);
            var nodeJ = model.GetNode(element.NodeJ);
            var material = model.GetMaterial(element.MaterialId);
            var section = model.GetSection(element.SectionId);

            var elementDisplacementsGlobal = Vector<double>.Build.Dense(element.GlobalDofs.Count);
            for (int i = 0; i < element.GlobalDofs.Count; i++)
            {
                var dof = element.GlobalDofs[i];
                var dofId = dofMap.GlobalIndexOf(dof);

                elementDisplacementsGlobal[i] = globalDisplacements[dofId];
            }

            elementForces.Add(
                element.ComputeInternalForces(nodeI, nodeJ, material, section, elementDisplacementsGlobal));
        }

        return elementForces;
    }

    private List<TResult> BuildNodalResults<TResult>(
        Vector<double> values,
        GlobalDofIndexMap dofMap,
        Func<int, bool> includeNode,
        Func<int, double, double, double, TResult> createResult)
    {
        var componentsGroupedByNode = new SortedDictionary<int, Dictionary<DofType, double>>();
        for (int i = 0; i < values.Count; i++)
        {
            var dof = dofMap.GetDof(i);
            if (!includeNode(dof.NodeId))
                continue;

            if (!componentsGroupedByNode.TryGetValue(dof.NodeId, out var valuesForNode))
            {
                valuesForNode = new Dictionary<DofType, double>();
                componentsGroupedByNode[dof.NodeId] = valuesForNode;
            }
            valuesForNode.Add(dof.Type, values[i]);
        }


        var results = new List<TResult>();
        foreach (var (nodeId, components) in componentsGroupedByNode)
        {
            results.Add(
                createResult(
                    nodeId,
                    components.GetValueOrDefault(DofType.Ux),
                    components.GetValueOrDefault(DofType.Uy),
                    components.GetValueOrDefault(DofType.Rz)));
        }

        return results;
    }
}
