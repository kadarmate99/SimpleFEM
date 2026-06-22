using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Preprocessing;

internal class Assembler
{
    internal GlobalSystem Assemble(FemModel model, GlobalDofIndexMap dofMap)
    {
        var globalK = BuildGlobalStiffnessMatrix(model, dofMap);
        var globalF = BuildGlobalForceVector(model, dofMap);

        return new GlobalSystem(globalK, globalF);
    }

    private Matrix<double> BuildGlobalStiffnessMatrix(FemModel model, GlobalDofIndexMap dofMap)
    {
        var globalK = Matrix<double>.Build.Dense(dofMap.ActiveDofCount, dofMap.ActiveDofCount);
        foreach (var element in model.Elements)
        {
            var nodeI = model.GetNode(element.NodeI);
            var nodeJ = model.GetNode(element.NodeJ);
            var material = model.GetMaterial(element.MaterialId);
            var section = model.GetSection(element.SectionId);

            var elementK = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);

            var elementDofLocalToGlobal = new int[element.GlobalDofs.Count];
            for (int i = 0; i < elementDofLocalToGlobal.Length; i++)
            {
                elementDofLocalToGlobal[i] = dofMap.GlobalIndexOf(element.GlobalDofs[i]);
            }

            ScatterAddElementContribution(globalK, elementDofLocalToGlobal, elementK);
        }

        return globalK;
    }

    private Vector<double> BuildGlobalForceVector(FemModel model, GlobalDofIndexMap dofMap)
    {
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

        return F;
    }

    private void ScatterAddElementContribution(
        Matrix<double> globalK,
        int[] elementDofLocalToGlobal,
        Matrix<double> elementK)
    {
        for (int i = 0; i < elementK.RowCount; i++)
            for (int j = 0; j < elementK.ColumnCount; j++)
                globalK[elementDofLocalToGlobal[i], elementDofLocalToGlobal[j]] +=
                    elementK[i, j];
    }
}
