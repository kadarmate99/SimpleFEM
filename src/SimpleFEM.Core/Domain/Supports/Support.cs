namespace SimpleFEM.Core.Domain.Supports;

/// <summary>
/// Restraint per DOF direction of one node.
/// A null restraint means the DOF is free.
/// </summary>
public sealed class Support
{
    public int NodeId { get; }
    public Restraint? Ux { get; }
    public Restraint? Uy { get; }
    public Restraint? Rz { get; }

    public Support(int nodeId, Restraint? ux = null, Restraint? uy = null, Restraint? rz = null)
    {
        NodeId = nodeId;
        Ux = ux;
        Uy = uy;
        Rz = rz;
    }

    internal IEnumerable<RestrainedDof> GetRestrainedDofs()
    {
        List<RestrainedDof> restrainedDofs = [];

        if (Ux is not null)
            restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Ux), Ux));

        if (Uy is not null)
            restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Uy), Uy));

        if (Rz is not null)
            restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Rz), Rz));

        return restrainedDofs;
    }
}
