using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Loads
{
    public record NodalLoad(
        int NodeId,
        double Fx,
        double Fy,
        double Mz)
    {
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
}