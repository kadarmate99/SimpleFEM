using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Elements;

public interface ILineElement
{
    int Id { get; }
    int NodeI { get; }
    int NodeJ { get; }
    int MaterialId { get; }
    int SectionId { get; }

    IReadOnlyList<Dof> GlobalDofs { get; }
    Matrix<double> ComputeGlobalStiffnessMatrix(
         Node nodeI, Node nodeJ, Material material, CrossSection section);
    ElementInternalForceResult ComputeInternalForces(
                Node nodeI, Node nodeJ, Material material, CrossSection section,
                Vector<double> elementDisplacementsGlobal);
}
