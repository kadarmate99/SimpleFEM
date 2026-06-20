using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Preprocessing;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.PostProcessing
{
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
            var nodalDisplacements = new List<NodalDisplacementResult>();

            var displacementsByNodeId = new Dictionary<int, HashSet<DofValue>>();
            for (int i = 0; i < u.Count; i++)
            {
                var dof = dofMap.GetDof(i);

                if (!displacementsByNodeId.TryGetValue(dof.NodeId, out var displacements))
                {
                    displacements = new HashSet<DofValue>();
                    displacementsByNodeId[dof.NodeId] = displacements;
                }
                displacements.Add(new DofValue(dof, u[i]));
            }

            foreach (var node in displacementsByNodeId)
            {
                double uX = 0;
                double uY = 0;
                double rZ = 0;

                foreach (var displacement in node.Value)
                {
                    switch (displacement.Dof.Type)
                    {
                        case DofType.Ux:
                            uX = displacement.Value;
                            break;

                        case DofType.Uy:
                            uY = displacement.Value;
                            break;

                        case DofType.Rz:
                            rZ = displacement.Value;
                            break;

                        default:
                            throw new IndexOutOfRangeException(
                                $"Unexpected DOF type {displacement.Dof.Type}");
                    }
                }

                nodalDisplacements.Add(new NodalDisplacementResult(node.Key, uX, uY, rZ));
            }

            return nodalDisplacements;
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




            var reactionResults = new List<ReactionResult>();

            var reactionsByNodeId = new Dictionary<int, HashSet<DofValue>>();
            for (int i = 0; i < globalReactions.Count; i++)
            {
                var dof = dofMap.GetDof(i);

                if (!restrainedNodeIds.Contains(dof.NodeId))
                    continue; // free node, cant have a reaction

                if (!reactionsByNodeId.TryGetValue(dof.NodeId, out var reactions))
                {
                    reactions = new HashSet<DofValue>();
                    reactionsByNodeId[dof.NodeId] = reactions;
                }
                reactions.Add(new DofValue(dof, globalReactions[i]));
            }


            foreach (var node in reactionsByNodeId)
            {
                double rX = 0;
                double rY = 0;
                double mZ = 0;

                foreach (var reaction in node.Value)
                {
                    switch (reaction.Dof.Type)
                    {
                        case DofType.Ux:
                            rX = reaction.Value;
                            break;

                        case DofType.Uy:
                            rY = reaction.Value;
                            break;

                        case DofType.Rz:
                            mZ = reaction.Value;
                            break;

                        default:
                            throw new IndexOutOfRangeException(
                                $"Unexpected DOF type {reaction.Dof.Type}");
                    }
                }

                reactionResults.Add(new ReactionResult(node.Key, rX, rY, mZ));
            }

            return reactionResults;
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
    }
}
