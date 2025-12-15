using SimpleFEM.Core.Commands;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Services.GeometryService;
using System.Windows;
using System.Windows.Input;
using Line = SimpleFEM.Core.Models.Line;

namespace SimpleFEM.Core.Tools
{
    public class LineTool : IDrawingTool
    {
        private readonly IRepository<Line> _lineRepository;
        private readonly IRepository<Node> _nodeRepository;
        private readonly IGeometryService _geometryService;
        private readonly Commands.CommandManager _commandManager;
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
            IGeometryService geometryService,
            Commands.CommandManager commandManager)
        {
            _lineRepository = lineRepository;
            _nodeRepository = nodeRepository;
            _geometryService = geometryService;
            _commandManager = commandManager;
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
            {
                var deleteCommand = new DeleteNodeCommand(_nodeRepository, _firstNode!);
                _commandManager.ExecuteCommand(deleteCommand);
            }

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
                    var createLineCommand = new CreateLineCommand(
                        _lineRepository, _firstNode, _secondNode);
                    _commandManager.ExecuteCommand(createLineCommand);
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
            var createNodeCommand = new CreateNodeCommand(_nodeRepository, location.X, location.Y);
            _commandManager.ExecuteCommand(createNodeCommand);

            return createNodeCommand.CreatedNode!;
        }
    }
}
