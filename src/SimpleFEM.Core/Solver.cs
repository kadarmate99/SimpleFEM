using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Results;
using SimpleFEM.Domain.Entities;

namespace SimpleFEM.Core
{
    public class Solver : ISolver
    {
        public AnalysisResult Solve(Structure structure, LoadCase loadCase)
        {
            var nodeById = structure.Nodes.ToDictionary(n => n.Id);
            var materialById = structure.Materials.ToDictionary(m => m.Id);
            var sectionById = structure.Sections.ToDictionary(s => s.Id);

            // Map each node's GUID to its 0-based position in structure.Nodes.
            // This position determines the node's DOF indices: node at index i owns DOFs 2i and 2i+1.
            var nodeIndexById = structure.Nodes
                .Select((node, index) => (node.Id, index))
                .ToDictionary(x => x.Id, x => x.index);


            var K = BuildGlobalStiffnessMatrix(structure, nodeById, materialById, sectionById, nodeIndexById);
            var F = BuildForceVector(structure, loadCase, nodeIndexById);
            var freeDofs = GetFreeDofs(structure, nodeIndexById);

            var q = SolveDisplacements(structure, K, F, freeDofs);
            var R = ComputeReactions(K, F, q);
            var s = ComputeElementForces(q, structure, nodeById, materialById, sectionById, nodeIndexById);

            var nodalDisplacements = new List<NodalDisplacementResult>();
            var reactions = new List<ReactionResult>();
            var forces = new List<ElementForceResult>();

            foreach (var node in structure.Nodes)
            {
                int nodeIndex = nodeIndexById[node.Id];

                int xDofIndex = 2 * nodeIndex;
                int yDofIndex = 2 * nodeIndex + 1;

                var displacementX = q[xDofIndex];
                var displacementY = q[yDofIndex];

                nodalDisplacements.Add(new NodalDisplacementResult(node.Id, displacementX, displacementY));
            }

            for (int i = 0; i < structure.Supports.Count; i++)
            {
                var supp = structure.Supports[i];

                int nodeIndex = nodeIndexById[supp.NodeId];

                int xDofIndex = 2 * nodeIndex;
                int yDofIndex = 2 * nodeIndex + 1;

                var reactionX = R[xDofIndex];
                var reactionY = R[yDofIndex];

                reactions.Add(new ReactionResult(supp.Id, reactionX, reactionY));
            }

            for (int i = 0; i < structure.Elements.Count; i++)
            {
                var elem = structure.Elements[i];
                var elementForce = s[i];
                forces.Add(new ElementForceResult(elem.Id, elementForce));
            }


            return new AnalysisResult(
                structure.Id,
                loadCase.Id,
                nodalDisplacements,
                reactions,
                forces);
        }

        private Matrix<double> BuildGlobalStiffnessMatrix(
            Structure structure,
            Dictionary<Guid, Node> nodeById,
            Dictionary<Guid, Material> materialById,
            Dictionary<Guid, Section> sectionById,
            Dictionary<Guid, int> nodeIndexById)
        {
            var numDof = structure.NumDof;
            var K = Matrix<double>.Build.Dense(numDof, numDof);

            foreach (var elem in structure.Elements)
            {
                var elem2D = new TrussElement2D(
                    nodeById[elem.StartNodeId],
                    nodeById[elem.EndNodeId],
                    materialById[elem.MaterialId],
                    sectionById[elem.SectionId]);

                var ke = elem2D.GlobalStiffnessMatrix;

                // Scatter ke (4×4, local DOFs) into K (2N×2N, global DOFs).
                // Local DOF layout: [startX=0, startY=1, endX=2, endY=3]
                // Global DOF for node at index i:  x → 2i,  y → 2i+1
                int nodeIndex1 = nodeIndexById[elem.StartNodeId];
                int nodeIndex2 = nodeIndexById[elem.EndNodeId];
                // index = local DOF, value = global DOF
                int[] localToGlobal = {
                    2 * nodeIndex1, 2 * nodeIndex1 + 1,
                    2 * nodeIndex2, 2 * nodeIndex2 + 1 };

                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        K[localToGlobal[i], localToGlobal[j]] += ke[i, j];
            }

            return K;
        }

        private Vector<double> BuildForceVector(
            Structure structure,
            LoadCase loadCase,
            Dictionary<Guid, int> nodeIndexById)
        {
            var numDof = structure.NumDof;
            Vector<double> F = Vector<double>.Build.Dense(numDof);

            foreach (var load in loadCase.NodalLoads)
            {
                int nodeIndex = nodeIndexById[load.NodeId];
                F[2 * nodeIndex] += load.Force.Fx;
                F[2 * nodeIndex + 1] += load.Force.Fy;
            }
            return F;
        }

        private int[] GetFreeDofs(Structure structure, Dictionary<Guid, int> nodeIndexById)
        {
            var numDof = structure.NumDof;

            var isFixed = new bool[numDof]; // by default all false

            foreach (var support in structure.Supports)
            {
                int nodeIndex = nodeIndexById[support.NodeId];

                if (support.IsUxFixed)
                    isFixed[2 * nodeIndex] = true;
                if (support.IsUyFixed)
                    isFixed[2 * nodeIndex + 1] = true;
            }

            var freeDofs = Enumerable
                .Range(0, numDof)
                .Where(i => !isFixed[i]).ToArray();

            return freeDofs;
        }

        private Vector<double> SolveDisplacements(Structure structure, Matrix<double> K, Vector<double> F, int[] freeDofs)
        {
            int numDof = structure.NumDof;
            int nFree = freeDofs.Length;

            var Kff = Matrix<double>.Build.Dense(nFree, nFree);
            var Ff = Vector<double>.Build.Dense(nFree);

            for (int i = 0; i < nFree; i++)
            {
                Ff[i] = F[freeDofs[i]];
                for (int j = 0; j < nFree; j++)
                    Kff[i, j] = K[freeDofs[i], freeDofs[j]];
            }

            // K_ff · q_f = F_f
            Vector<double> qf = Kff.Solve(Ff);

            var q = Vector<double>.Build.Dense(numDof);
            for (int i = 0; i < nFree; i++)
                q[freeDofs[i]] = qf[i];

            return q;
        }

        private Vector<double> ComputeReactions(Matrix<double> K, Vector<double> F, Vector<double> q)
        {
            // R = K·q − F
            return K * q - F;
        }

        private Vector<double> ComputeElementForces(
            Vector<double> q,
            Structure structure,
            Dictionary<Guid, Node> nodeById,
            Dictionary<Guid, Material> materialById,
            Dictionary<Guid, Section> sectionById,
            Dictionary<Guid, int> nodeIndexById)
        {
            var forces = Vector<double>.Build.Dense(structure.Elements.Count);

            for (int i = 0; i < forces.Count; i++)
            {
                TrussElement elem = structure.Elements[i];
                var elem2D = new TrussElement2D(
                    nodeById[elem.StartNodeId],
                    nodeById[elem.EndNodeId],
                    materialById[elem.MaterialId],
                    sectionById[elem.SectionId]);

                int nodeIndex1 = nodeIndexById[elem2D.Start.Id];
                int nodeIndex2 = nodeIndexById[elem2D.End.Id];

                double L = elem2D.L;
                double c = elem2D.Direction[0];
                double s = elem2D.Direction[1];

                // Local displacement vector [u1x, u1y, u2x, u2y]
                var d = Vector<double>.Build.DenseOfArray(
                    [
                    q[2 * nodeIndex1], q[2 * nodeIndex1 + 1],
                    q[2 * nodeIndex2], q[2 * nodeIndex2 + 1],
                    ]);

                // Axial length change: u = [-c, -s, c, s] · d
                double deltaL = Vector<double>.Build.DenseOfArray([-c, -s, c, s]) * d;

                // F = EA * epsilon
                forces[i] = elem2D.EA * deltaL / L;
            }

            return forces;
        }
    }
}
