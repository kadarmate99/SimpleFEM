namespace SimpleFEM.Core.Domain.Supports
{
    public class Support
    {
        private readonly Restraint? _ux;
        private readonly Restraint? _uy;
        private readonly Restraint? _rz;

        public Support(int nodeId, Restraint? ux = null, Restraint? uy = null, Restraint? rz = null)
        {
            NodeId = nodeId;
            _ux = ux;
            _uy = uy;
            _rz = rz;
        }

        public int NodeId { get; }

        internal IEnumerable<RestrainedDof> GetRestrainedDofs()
        {
            List<RestrainedDof> restrainedDofs = [];

            if (_ux is not null) 
                restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Ux), _ux));

            if (_uy is not null) 
                restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Uy), _uy));

            if (_rz is not null)
                restrainedDofs.Add(new RestrainedDof(new Dof(NodeId, DofType.Rz), _rz));

            return restrainedDofs;
        }
    }
}
