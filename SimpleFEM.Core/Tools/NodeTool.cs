using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Services.GeometryService;
using System.Windows;

namespace SimpleFEM.Core.Tools
{
    public class NodeTool : IDrawingTool
    {
        private IRepository<Node> _nodeRepository;
        private IGeometryService _geometryService;

        public string Name => "Node";
        public bool InProgress => false; // No half done state. Always ready for next click
        public bool IsActive { get; private set; }

        public event EventHandler? StateChanged;

        public NodeTool(IRepository<Node> nodeRepository, IGeometryService geometryService)
        {
            _nodeRepository = nodeRepository;
            _geometryService = geometryService;
        }

        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            Reset();
        }

        public void Reset()
        {
            // No state to reset
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void HandleCanvasClick(Point location)
        {
            if (!IsActive) return;

            // Check if node already exists at this location
            var existingNode = _geometryService.FindNodeNear(
                location,
                _nodeRepository.GetAll());

            if (existingNode != null)
                return; // Don't create duplicate

            // Create new node
            var node = new Node
            {
                X = location.X,
                Z = location.Y
            };
            _nodeRepository.Add(node);

            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
