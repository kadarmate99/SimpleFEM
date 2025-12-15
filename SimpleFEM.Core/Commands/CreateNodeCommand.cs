using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Core.Commands
{
    public class CreateNodeCommand : ICommand
    {
        private readonly IRepository<Node> _nodeRepository; // receiver

        // Context data
        private readonly double _x;
        private readonly double _z;

        public Node? CreatedNode { get; private set; }

        public string Description => $"Create Node at ({_x:F2}, {_z:F2})";

        public CreateNodeCommand(IRepository<Node> nodeRepository, double x, double z)
        {
            _nodeRepository = nodeRepository;
            _x = x;
            _z = z;
        }

        public void Execute()
        {
            var node = new Node
            {
                X = _x,
                Z = _z
            };

            _nodeRepository.Add(node);
            CreatedNode = node;
        }

        public void Undo()
        {
            if (CreatedNode == null)
                throw new InvalidOperationException("Cannot undo: Node was not created.");

            _nodeRepository.Delete(CreatedNode.Id);
        }
    }
}
