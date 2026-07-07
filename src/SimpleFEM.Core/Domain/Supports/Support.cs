namespace SimpleFEM.Core.Domain.Supports;

/// <summary>
/// Restraint per DOF direction of one node.
/// A null restraint means the DOF is free.
/// </summary>
public sealed record Support
{
    public int NodeId { get; }
    public Restraint? Ux { get; }
    public Restraint? Uy { get; }
    public Restraint? Rz { get; }

    public Support(int nodeId, Restraint? ux = null, Restraint? uy = null, Restraint? rz = null)
    {
        if (ux is null && uy is null && rz is null)
            throw new ArgumentException("A support must restrain at least one DOF.");

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
