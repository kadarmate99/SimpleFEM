using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Services.GeometryService;
using System.Windows;
using Line = SimpleFEM.Core.Models.Line;

namespace SimpleFEM.Core.Tools
{
    public class LineTool : IDrawingTool
    {
        private readonly IRepository<Line> _lineRepository;
        private readonly IRepository<Node> _nodeRepository;
        private readonly IGeometryService _geometryService;

        private Node? _firstNode;
        private Node? _secondNode;

        public string Name => "Line";

        //The Line tool is in the definition process if only the first node was created yet.
        public bool InProgress => (_firstNode != null && _secondNode == null);
        public bool IsActive { get; private set; }

        public event EventHandler? StateChanged;

        public LineTool(
            IRepository<Line> lineRepository,
            IRepository<Node> nodeRepository,
            IGeometryService geometryService)
        {
            _lineRepository = lineRepository;
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
            if (InProgress)
                _nodeRepository.Delete(_firstNode!.Id);

            _firstNode = null;
            _secondNode = null;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void HandleCanvasClick(Point location)
        {
            if (!IsActive) return;

            if (_firstNode is null)
            {
                // First Click
                _firstNode = FindOrCreateNode(location);
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Second Click
                _secondNode = FindOrCreateNode(location);

                if (_firstNode.Id != _secondNode.Id)
                {
                    CreateLine(_firstNode, _secondNode);
                }

                Reset();
            }

        }

        private Node FindOrCreateNode(Point location)
        {
            // Check if node already exists at this location
            var existingNode = _geometryService.FindNodeNear(
                location,
                _nodeRepository.GetAll());

            if (existingNode != null)
                return existingNode;

            // No existing node, create new
            var nodeModel = new Node
            {
                X = location.X,
                Z = location.Y
            };
            _nodeRepository.Add(nodeModel);
            return nodeModel;
        }

        private Line CreateLine(Node start, Node end)
        {
            var lineModel = new Line
            {
                INode = start,
                JNode = end
            };

            _lineRepository.Add(lineModel);
            return lineModel;
        }
    }
}
