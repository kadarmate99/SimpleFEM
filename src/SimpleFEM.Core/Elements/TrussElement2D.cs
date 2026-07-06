using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Elements;

/// <summary>
/// 2-node axial-force-only element in the XY plane 
/// </summary>
public class TrussElement2D : Element
{
    private readonly Dof[] _globalDofs;

    public TrussElement2D(int id, int nodeI, int nodeJ, int materialId, int sectionId)
        :base(id, [nodeI, nodeJ], materialId, sectionId)
    {
        _globalDofs =
            [
            new Dof(nodeI, DofType.Ux), new Dof(nodeI, DofType.Uy),
            new Dof(nodeJ, DofType.Ux), new Dof(nodeJ, DofType.Uy),
            ];
    }

    public int NodeI => NodeIds[0];
    public int NodeJ => NodeIds[1];
    internal override IReadOnlyList<Dof> GlobalDofs => _globalDofs;

    internal override Matrix<double> ComputeGlobalStiffnessMatrix(ElementContext context)
    {
        var kLoc = ComputeLocalStiffnessMatrix(context);
        var transform = ComputeTransformationMatrix(context);

        return transform.Transpose() * kLoc * transform;
    }

    internal override ElementInternalForceResult ComputeInternalForces(
                ElementContext context,
                Vector<double> elementDisplacementsGlobal)
    {
        var transform = ComputeTransformationMatrix(context);
        var uBar = transform * elementDisplacementsGlobal;
        double k = context.Material.E * context.Section.A / Length(context);
        double N = k * (uBar[2] - uBar[0]); // tension positive
        return new ElementInternalForceResult(Id, N, 0, 0);
    }

    internal Matrix<double> ComputeLocalStiffnessMatrix(
        ElementContext context)
    {

        double k = context.Material.E * context.Section.A / Length(context);

        return Matrix<double>.Build.DenseOfArray(new double[,]
        {
            {  k, 0, -k, 0 },
            {  0, 0,  0, 0 },
            { -k, 0,  k, 0 },
            {  0, 0,  0, 0 }
        });
    }

    private Matrix<double> ComputeTransformationMatrix(ElementContext context)
    {
        (double c, double s) = DirectionCosines(context);

        return Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { c, s, 0, 0 },
            { -s, c, 0, 0 },
            { 0, 0, c, s },
            { 0, 0, -s, c }
        });
    }

    private double Length(ElementContext context)
    {
        var (nodeI, nodeJ) = (context.Nodes[0], context.Nodes[1]);
        double dx = nodeJ.X - nodeI.X;
        double dy = nodeJ.Y - nodeI.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private (double c, double s) DirectionCosines(ElementContext context)
    {
        var (nodeI, nodeJ) = (context.Nodes[0], context.Nodes[1]);
        double length = Length(context);
        double dx = nodeJ.X - nodeI.X;
        double dy = nodeJ.Y - nodeI.Y;
        return (dx / length, dy / length);
    }
}
