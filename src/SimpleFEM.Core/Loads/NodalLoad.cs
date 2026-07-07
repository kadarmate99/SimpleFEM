using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Loads;

public record NodalLoad
{
    public int NodeId { get; }
    public double Fx { get; }
    public double Fy { get; }
    public double Mz { get; }
    public NodalLoad(
        int nodeId,
        double fx,
        double fy,
        double mz)
    {
        if (fx == 0 && fy == 0 && mz == 0)
            throw new ArgumentException("A nodal load must contain at least one load value.");

        NodeId = nodeId;
        Fx = fx;
        Fy = fy;
        Mz = mz;
    }

    internal IEnumerable<DofValue> GetLoadsOnDofs()
    {
        var loadsOnDofs = new List<DofValue>();

        if (Fx != 0)
            loadsOnDofs.Add(new DofValue(new Dof(NodeId, DofType.Ux), Fx));

        if (Fy != 0)
            loadsOnDofs.Add(new DofValue(new Dof(NodeId, DofType.Uy), Fy));

        if (Mz != 0)
            loadsOnDofs.Add(new DofValue(new Dof(NodeId, DofType.Rz), Mz));

        return loadsOnDofs;
    }

}