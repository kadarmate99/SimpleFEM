using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Preprocessing
{
    internal class Assembler
    {
        internal GlobalSystem Assemble(FemModel model, GlobalDofIndexMap dofMap)
        {
            var K = Matrix<double>.Build.Dense(dofMap.ActiveDofCount, dofMap.ActiveDofCount);
            foreach (var element in model.Elements)
            {
                var nodeI = model.GetNode(element.NodeI);
                var nodeJ = model.GetNode(element.NodeJ);
                var material = model.GetMaterial(element.MaterialId);
                var section = model.GetSection(element.SectionId);

                var kElemGlob = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);

                var elementDofLocalToGlobal = new int[element.GlobalDofs.Count];
                for (int i = 0; i < elementDofLocalToGlobal.Length; i++)
                {
                    elementDofLocalToGlobal[i] = dofMap.GlobalIndexOf(element.GlobalDofs[i]);
                }

                for (int i = 0; i < kElemGlob.RowCount; i++)
                    for (int j = 0; j < kElemGlob.ColumnCount; j++)
                        K[elementDofLocalToGlobal[i], elementDofLocalToGlobal[j]] +=
                            kElemGlob[i, j];
            }

            var F = Vector<double>.Build.Dense(dofMap.ActiveDofCount);
            foreach (var load in model.Loads)
            {
                var loadsOnDofs = load.GetLoadsOnDofs().ToArray();
                var dofCount = loadsOnDofs.Length;

                for (int i = 0; i < dofCount; i++)
                {
                    var (dof, value) = loadsOnDofs[i];

                    // if there is a load on an inactive dof (it has no stiffness)
                    // dofMap.GlobalIndexOf will throw an exception.
                    // this is modeling error. validation should have caught this...
                    var dofGlobIndex = dofMap.GlobalIndexOf(dof);

                    F[dofGlobIndex] += value;
                }
            }

            return new GlobalSystem(K, F);
        }
    }
}
