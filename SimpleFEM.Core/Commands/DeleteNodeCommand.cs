using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Core.Commands
{
    public class DeleteNodeCommand : ICommand
    {
        private readonly IRepository<Node> _nodeRepository;
        private readonly Node _deletedNode;

        public string Description => $"Delete Node {_deletedNode.Id}";

        public DeleteNodeCommand(IRepository<Node> nodeRepository, Node node)
        {
            _nodeRepository = nodeRepository;
            _deletedNode = node;
        }

        public void Execute()
        {
            _nodeRepository.Delete(_deletedNode.Id);
        }

        public void Undo()
        {
            _nodeRepository.Add(_deletedNode);
        }
    }
}
