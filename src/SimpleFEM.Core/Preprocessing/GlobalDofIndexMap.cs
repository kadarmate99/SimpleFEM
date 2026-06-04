using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Elements;

namespace SimpleFEM.Core.Preprocessing
{
    internal class GlobalDofIndexMap
    {
        private static readonly DofType[] NodeDofOrder = Enum.GetValues<DofType>();

        private readonly Dictionary<Dof, int> _indexByDof = [];
        private readonly List<Dof> _dofsByIndex = [];

        internal GlobalDofIndexMap(IReadOnlyList<Node> nodes, IEnumerable<ILineElement> elements)
        {
            // inactive DOFs are excluded:
            // an inactive DOF is not relevant in the system because nothing in the model provides
            // stiffness for that DOF.
            // adding inactive DOFs would result in empty rows in the global stiffness matrix (singularity)
            // and these empty rows would have to be eliminated anyway so the system is solvable.
            // thats why i dont add these.

            // filter active DOFs
            var activeTypesByNodeId = new Dictionary<int, HashSet<DofType>>();
            foreach (var element in elements)
            {
                foreach (var dof in element.GlobalDofs)
                {
                    if (!activeTypesByNodeId.TryGetValue(dof.NodeId, out var types))
                    {
                        types = new HashSet<DofType>();
                        activeTypesByNodeId[dof.NodeId] = types;
                    }
                    types.Add(dof.Type);
                }
            }

            // build DOF global index dictionary from active DOFs
            int currentDofIndex = 0;
            foreach (var node in nodes)
            {
                if (!activeTypesByNodeId.TryGetValue(node.Id, out var types))
                    continue; // this node is not referenced by any element, it has no active DOFs

                foreach (var type in NodeDofOrder)
                {
                    if (!types.Contains(type))
                        continue; // skip DOF types this nodes elements dont have (inactive)

                    var dof = new Dof(node.Id, type);
                    _indexByDof[dof] = currentDofIndex;
                    _dofsByIndex.Add(dof);
                    currentDofIndex++;
                }
            }
        }

        internal int ActiveDofCount => _indexByDof.Count;

        internal int GlobalIndexOf(Dof dof) =>
            _indexByDof.TryGetValue(dof, out var index)
                ? index
                : throw new ArgumentException(
                    $"DOF {dof} is inactive and has no global DOF index. " +
                    $"Nothing in the model provides stiffness for it", nameof(dof));

        internal Dof GetDof(int globalDofIndex)
        {
            return _dofsByIndex[globalDofIndex];
        }
    }
}
