using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Elements;

public class TrussElement2D : ILineElement
{
    public int Id { get; }
    public int NodeI { get; }
    public int NodeJ { get; }
    public int MaterialId { get; }
    public int SectionId { get; }
    public IReadOnlyList<Dof> GlobalDofs =>
        [
            new Dof(NodeI, DofType.Ux), new Dof(NodeI, DofType.Uy),
            new Dof(NodeJ, DofType.Ux), new Dof(NodeJ, DofType.Uy),
        ];

    public TrussElement2D(int id, int nodeI, int nodeJ, int materialId, int sectionId)
    {
        Id = id;
        NodeI = nodeI;
        NodeJ = nodeJ;
        MaterialId = materialId;
        SectionId = sectionId;
    }

    public Matrix<double> ComputeGlobalStiffnessMatrix(
        Node nodeI, Node nodeJ, Material material, CrossSection section)
    {
        var kLoc = ComputeLocalStiffnessMatrix(nodeI, nodeJ, material, section);
        var transform = ComputeTransformationMatrix(nodeI, nodeJ);

        return transform.Transpose() * kLoc * transform;
    }

    public ElementInternalForceResult ComputeInternalForces(
                Node nodeI, Node nodeJ, Material material, CrossSection section,
                Vector<double> elementDisplacementsGlobal)
    {
        var transform = ComputeTransformationMatrix(nodeI, nodeJ);
        var uBar = transform * elementDisplacementsGlobal;
        double k = material.E * section.A / Length(nodeI, nodeJ);
        double N = k * (uBar[2] - uBar[0]); // tension positive
        return new ElementInternalForceResult(Id, N, 0, 0);
    }

    private Matrix<double> ComputeLocalStiffnessMatrix(
        Node nodeI, Node nodeJ, Material material, CrossSection section)
    {

        double k = material.E * section.A / Length(nodeI, nodeJ);

        return Matrix<double>.Build.DenseOfArray(new double[,]
        {
            {  k, 0, -k, 0 },
            {  0, 0,  0, 0 },
            { -k, 0,  k, 0 },
            {  0, 0,  0, 0 }
        });
    }

    private Matrix<double> ComputeTransformationMatrix(Node nodeI, Node nodeJ)
    {
        (double c, double s) = DirectionCosines(nodeI, nodeJ);

        return Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { c, s, 0, 0 },
            { -s, c, 0, 0 },
            { 0, 0, c, s },
            { 0, 0, -s, c }
        });
    }

    private double Length(Node nodeI, Node nodeJ)
    {
        double dx = nodeJ.X - nodeI.X;
        double dy = nodeJ.Y - nodeI.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private (double c, double s) DirectionCosines(Node nodeI, Node nodeJ)
    {
        double length = Length(nodeI, nodeJ);
        double dx = nodeJ.X - nodeI.X;
        double dy = nodeJ.Y - nodeI.Y;
        return (dx / length, dy / length);
    }
}
